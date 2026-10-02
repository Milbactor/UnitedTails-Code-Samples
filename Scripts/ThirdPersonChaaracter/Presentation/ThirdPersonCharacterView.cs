using System;
using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public class ThirdPersonCharacterView : MonoBehaviour, IThirdPersonCharacterView
    {
        [SerializeField] private float _animSpeedMultiplier = 1f;
        [Header("Value normalized to 1 when bringing to animator's falling speed")]
        [SerializeField] private float _maxFallSpeedForAnim = 8f;//max value which is normalized to 1 when bring to Animator range value -1~1. 8 means 1 in animator

        private CharacterMovementSettings _characterMovementSetting;

        [SerializeField] private  Rigidbody _rigidbody;
        [SerializeField] private Animator _animator;
        [SerializeField] private CapsuleCollider _capsule;

        private float _forwardAmount;
        private float _turnAmount;
        private bool _isGrounded;
        private float _pendingYawDelta;

        public const float k_Half = 0.5f;
        private const float animatorDeadZone = 0.09f;

        public Rigidbody Rigidbody { get => _rigidbody; }
        public Animator Animator { get => _animator; }
        public CapsuleCollider Capsule { get => _capsule; }

        private float _smoothedTurnAmount;
        private float _turnSmoothVelocity;
      
        public IObservable<Unit> OnUpdate =>
         Observable.EveryUpdate().AsUnitObservable();
        public IObservable<Unit> OnFixedUpdate =>
         Observable.EveryFixedUpdate().AsUnitObservable();

        private readonly Subject<Unit> _onSpinJumpFinished = new Subject<Unit>();
        public IObservable<Unit> OnSpinJumpFinished => _onSpinJumpFinished;

        private readonly Subject<Unit> _onJumpFinished = new Subject<Unit>();
        public IObservable<Unit> OnJumpFinished => _onJumpFinished;

        private void Awake()
        {
            _animator = _animator == null ? GetComponent<Animator>() : _animator;
            _rigidbody = _rigidbody == null ? GetComponent<Rigidbody>() : _rigidbody;
            _capsule = _capsule == null ? GetComponent<CapsuleCollider>() : _capsule;
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX 
                | RigidbodyConstraints.FreezeRotationY 
                | RigidbodyConstraints.FreezeRotationZ;

            _characterMovementSetting = GetComponent<CharacterSetting>().CharacterMovementSettings;
            if (_characterMovementSetting == null) { Debug.LogError("CharacterMovementSettings is missing"); }
        }

        private void FixedUpdate()
        {
            if (Mathf.Abs(_pendingYawDelta) < 0.001f)
                return;

            Quaternion rotation =
                _rigidbody.rotation *
                Quaternion.Euler(0f, _pendingYawDelta, 0f);

            _rigidbody.MoveRotation(rotation);
            _pendingYawDelta = 0f;
        }

        public void Move(Vector3 direction, Vector3 groundNormal)
        {
            if (direction.magnitude > 1f)
                direction.Normalize();

            direction = Vector3.ProjectOnPlane(direction, groundNormal);

            var localDirection =
                transform.InverseTransformDirection(direction);

            _turnAmount =
                Mathf.Atan2(localDirection.x, localDirection.z);

            _forwardAmount = localDirection.z;
            _characterMovementSetting.LateralAmount = localDirection.x;

            if (Mathf.Abs(_turnAmount) < animatorDeadZone)
                _turnAmount = 0f;

            if (Mathf.Abs(_forwardAmount) < animatorDeadZone)
                _forwardAmount = 0f;

            ApplyExtraTurnRotation();
        }

        public void UpdateAnimator(Vector3 direction, bool isGrounded)
        {
            _animator.SetFloat("Forward", _forwardAmount, 0.1f, Time.deltaTime);
            _animator.SetFloat("Turn", _turnAmount, 0.1f, Time.deltaTime);
            _animator.SetBool("OnGround", isGrounded);
            if (!isGrounded) {
                _animator.SetFloat("Jump", Mathf.Clamp(Rigidbody.velocity.y/ _maxFallSpeedForAnim, -1f, 1f));
            }
           
            float runCycle =
                    Mathf.Repeat(
                        Animator.GetCurrentAnimatorStateInfo(0).normalizedTime + _characterMovementSetting.RunCycleLegOffset, 1);
            float jumpLeg = (runCycle < k_Half ? 1 : -1) * _forwardAmount;
            if (isGrounded) {
                _animator.SetFloat("JumpLeg", jumpLeg);
            }

            if (isGrounded && direction.sqrMagnitude > 0.0001f) {
                _animator.speed = _animSpeedMultiplier;
            }
            else {
                Animator.speed = 1;
            }
        }

        public void HandleAirborneMovement()
        {
            Vector3 extraGravityAcceleration =
                Physics.gravity * (_characterMovementSetting.GravityMultiplier - 1f);

            _rigidbody.AddForce(
                extraGravityAcceleration,
                ForceMode.Acceleration
            );

            ApplyAirMovement();

            Vector3 velocity = _rigidbody.velocity;

            if (velocity.y < -_characterMovementSetting.MaxFallSpeed)
            {
                velocity.y = -_characterMovementSetting.MaxFallSpeed;
                _rigidbody.velocity = velocity;
            }
        }

        public void HandleGroundedMovement()
        {
            if (!_isGrounded)
                return;

            _rigidbody.velocity = new Vector3(
                _rigidbody.velocity.x,
                _characterMovementSetting.JumpPower,
                _rigidbody.velocity.z
            );

            SetApplyRootMotion(false);
            StartJump();
        }

        public void HandleSpinJumpMovement()
        {
            if (_isGrounded)
                return;

            _rigidbody.velocity = new Vector3(
                _rigidbody.velocity.x,
                _characterMovementSetting.SpinJumpPower,
                _rigidbody.velocity.z
            );

            SetApplyRootMotion(false);
            StartSpinJump();
        }

        private void ApplyAirMovement()
        {
            float forwardInput = Mathf.Max(0f, _forwardAmount);

            Vector3 targetHorizontalVelocity =
                transform.forward * forwardInput * _characterMovementSetting.AirForwardMaxSpeed
                + transform.right * _characterMovementSetting.LateralAmount * _characterMovementSetting.AirLateralMaxSpeed;

            Vector3 velocity = _rigidbody.velocity;

            Vector3 currentHorizontalVelocity =
                new Vector3(velocity.x, 0f, velocity.z);

            Vector3 newHorizontalVelocity =
                Vector3.MoveTowards(
                    currentHorizontalVelocity,
                    targetHorizontalVelocity,
                    _characterMovementSetting.AirForwardAcceleration * Time.fixedDeltaTime
                );

            velocity.x = newHorizontalVelocity.x;
            velocity.z = newHorizontalVelocity.z;

            _rigidbody.velocity = velocity;
        }

        public void ApplyExtraTurnRotation()
        {
            _smoothedTurnAmount = Mathf.SmoothDamp(
                _smoothedTurnAmount,
                _turnAmount,
                ref _turnSmoothVelocity,
                _characterMovementSetting.TurnSmoothTime
            );

            if (Mathf.Abs(_smoothedTurnAmount) < 0.01f)
                return;

            float turnSpeed = Mathf.Lerp(
               _characterMovementSetting.StationaryTurnSpeed,
                _characterMovementSetting.MovingTurnSpeed,
                Mathf.Abs(_forwardAmount)
            );

            _pendingYawDelta +=
                _smoothedTurnAmount * turnSpeed * Time.deltaTime;
        }

        public void OnAnimatorMove()
        {
            // we implement this function to override the default root motion.
            // this allows us to modify the positional speed before it's applied.
            if (_isGrounded && Time.deltaTime > 0)
            {
                Vector3 v = (_animator.deltaPosition * _characterMovementSetting.MoveSpeedMultiplier) / Time.deltaTime;

                // we preserve the existing y part of the current velocity.
                v.y = Rigidbody.velocity.y;
                Rigidbody.velocity = v;
            }
        }

        public void SetApplyRootMotion(bool isRootMotion)
        {
            _animator.applyRootMotion = isRootMotion;
        }

        public void SetGroundState(bool grounded)
        {
            _isGrounded = grounded;
        }

        public Vector3 TransformDirection(Vector3 localDirection)
        {
            return transform.TransformDirection(localDirection);
        }

        void DrawCheckGroundDistance(float groundCheckDistance)
        {
#if UNITY_EDITOR
            // helper to visualise the ground check ray in the scene view
            Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * groundCheckDistance));
#endif
        }

        public void StartJump()
        {
            _animator.SetFloat("Jump", 1f);
        }

        public void StartSpinJump()
        {
            _animator.SetFloat("Jump", 1f);
            _animator.SetTrigger("TriggerSpinJump");
        }

        public void JumpFinished()
        {
            _onJumpFinished.OnNext(Unit.Default);
        }

        public void SpinJumpFinished()
        {
            _onSpinJumpFinished.OnNext(Unit.Default);
        }
    }
}