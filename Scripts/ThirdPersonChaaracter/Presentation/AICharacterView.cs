using System;
using UniRx;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace WhiteKNight
{
    public class AICharacterView :  MonoBehaviour, IAICharacterView, ICharacterLifeCycle
    {
        [SerializeField] private float _movingTurnSpeed = 360f;
        [SerializeField] private float _stationaryTurnSpeed = 180f;
        [SerializeField] private float _jumpPower = 12f;
        [SerializeField] private float _spinJumpPower = 5f;
        [SerializeField] private float _approachSpinJumpPower = 8f;
        [SerializeField] private float _approachJumpPower = 8f;
        [Range(1f, 4f)][SerializeField] private float _GravityMultiplier = 2f;
        [SerializeField] private float _RunCycleLegOffset = 0.2f;
        [SerializeField] private float _maxFallSpeedForAnim = 8f;

        [Header("Chase")]
        [SerializeField] private float _minChaseDistance = 3.1f;
        [SerializeField] private float _maxChaseDistance = 6f;
        [SerializeField] private float _maxChaseSpeed = 10f;
        [SerializeField] private float _rotationSmooth = 10f;

        [Header("References")]
        [SerializeField] private Animator _animator;
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private CapsuleCollider _capsule;
        [SerializeField] private Rigidbody _rigidbody;

        private float _forwardAmount;
        private float _turnAmount;
        private bool _isGrounded;
        private Vector3 _targetPosition;


        public const float k_Half = 0.5f;
        private const float AnimationDeadZone = 0.01f;

        public Animator Animator => _animator;
        public CapsuleCollider Capsule => _capsule;

        public IObservable<Unit> OnUpdate =>
            Observable.EveryUpdate().AsUnitObservable();

        private readonly Subject<Unit> _onSpinJumpFinished = new Subject<Unit>();
        public IObservable<Unit> OnSpinJumpFinished => _onSpinJumpFinished;

        private readonly Subject<Unit> _onJumpFinished = new Subject<Unit>();
        public IObservable<Unit> OnJumpFinished => _onJumpFinished;

        [Inject]
        public void Construct(CharacterDeathNotifier characterDeathNotifier)
        {
            characterDeathNotifier.Register(this);
        }

        private void Awake()
        {
            _animator = _animator == null ? GetComponent<Animator>() : _animator;
            _capsule = _capsule == null ? GetComponent<CapsuleCollider>() : _capsule;
            _rigidbody = _rigidbody == null ? GetComponent<Rigidbody>() : _rigidbody;
            _agent = _agent == null ? GetComponent<NavMeshAgent>() : _agent;
        }


        private void Update()
        {
            if (_isDead) return;
  
            if (!_agent.enabled) return;

            UpdateChaseSpeed();

            Vector3 toTarget = _targetPosition - transform.position;
            toTarget.y = 0f;

            Vector3 moveDir = toTarget.normalized;

            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * 10f
            );
        }

        private void UpdateChaseSpeed()
        {
            Vector3 flatTarget = _targetPosition;
            flatTarget.y = transform.position.y;

            float distanceToTarget = Vector3.Distance(flatTarget, transform.position);
            float t = Mathf.InverseLerp(_minChaseDistance, _maxChaseDistance, distanceToTarget);

            float speed = Mathf.Lerp(0f, _maxChaseSpeed, t);
            if (speed < 0.05f) { speed = 0f; }
            _agent.speed = speed;
        }

        public void Move(Vector3 direction, Vector3 groundNormal, AIMovementMode aIMovementMode)
        {
            if (aIMovementMode == AIMovementMode.NavMesh)
            {
                UpdateChaseSpeed();

                // 1. Agentのシミュレーション上の位置を更新
                if (_agent.desiredVelocity.sqrMagnitude > 0.01f)
                {
                    _agent.nextPosition = transform.position
                        + _agent.desiredVelocity * Time.deltaTime;
                }
           
                Vector3 targetPos = _agent.nextPosition;

                // --- 修正ポイント：高速追従ロジック ---
                float dist = Vector3.Distance(transform.position, targetPos);

                // 0.001m（1mm）以下の微細なブレは無視して、無駄な座標更新をカット
                if (dist > 0.001f)
                {
                    // スピード7なら、少し余裕を持って「スピード10」くらいの力で追いかける
                    // これで「遅れ」によるガタつきを防ぎます
                    float followSpeed = Mathf.Max(_agent.speed, 10f);
                    transform.position = Vector3.MoveTowards(transform.position, targetPos, followSpeed * Time.deltaTime);
                }

                // 2. 位置の同期（これは必須）
                _agent.nextPosition = transform.position;

                // 3. 回転処理（高速時は少し粘り気を持たせる）
                Vector3 desired = _agent.desiredVelocity;
                desired.y = 0f;

                if (desired.sqrMagnitude > 0.001f)
                {
                    Vector3 localDesired = transform.InverseTransformDirection(desired.normalized);
                    _turnAmount = QuantizeTurn(localDesired.x);

                    // スピード7に合わせて回転の滑らかさを調整
                    transform.rotation = Quaternion.Slerp(
                        transform.rotation,
                        Quaternion.LookRotation(desired.normalized),
                        Time.deltaTime * _rotationSmooth
                    );
                }
                else
                {
                    _turnAmount = 0f;
                }

                _forwardAmount = GetNavMeshForward();
            }
            else // AIMovementMode.Physics
            {
                if (direction.magnitude > 1f)
                {
                    direction.Normalize();
                }

                direction = Vector3.ProjectOnPlane(direction, groundNormal);
                direction = transform.InverseTransformDirection(direction);

                _turnAmount = Mathf.Atan2(direction.x, direction.z);
                _forwardAmount = direction.z;

                ApplyExtraTurnRotation();
            }

            // 値をクリーンにしてアニメーターに備える
            _turnAmount = CleanAnimatorValue(_turnAmount);
            _forwardAmount = CleanAnimatorValue(_forwardAmount);
        }

        private float GetNavMeshForward()
        {
            if (!_agent.enabled) return 0f;

            float speed01 = Mathf.Clamp01(_agent.speed / _maxChaseSpeed);

            if (speed01 < 0.05f) return 0f;
            if (speed01 < 0.5f) return 0.5f;
            return 1f;
        }

        private float QuantizeTurn(float turn)
        {
            if (turn < -0.75f) return -1f;
            if (turn < -0.25f) return -0.5f;
            if (turn > 0.75f) return 1f;
            if (turn > 0.25f) return 0.5f;
            return 0f;
        }

        private float CleanAnimatorValue(float value)
        {
            value = Mathf.Clamp(value, -1f, 1f);

            if (Mathf.Abs(value) < AnimationDeadZone)
            {
                return 0f;
            }

            return Mathf.Round(value * 100f) / 100f;
        }

        public void UpdateAnimator(Vector3 move, bool isGrounded)
        {
            float forward = CleanAnimatorValue(_forwardAmount);
            float turn = CleanAnimatorValue(_turnAmount);

            _animator.SetFloat("Forward", forward);
            _animator.SetFloat("Turn", turn);
            _animator.SetBool("OnGround", isGrounded);

            if (!isGrounded)
            {
                float verticalSpeed = _agent.enabled
                    ? _agent.velocity.y
                    : _rigidbody.velocity.y;

                _animator.SetFloat(
                    "Jump",
                    Mathf.Clamp(verticalSpeed / _maxFallSpeedForAnim, -1f, 1f)
                );
            }

            float jumpLeg = 0f;

            if (!isGrounded && Mathf.Abs(forward) > AnimationDeadZone)
            {
                float runCycle = Mathf.Repeat(
                    Animator.GetCurrentAnimatorStateInfo(0).normalizedTime + _RunCycleLegOffset,
                    1f
                );

                jumpLeg = (runCycle < k_Half ? 1f : -1f) * forward;
            }

            _animator.SetFloat("JumpLeg", jumpLeg);
        }

        public void HandleAirborneMovement()
        {
            Vector3 extraGravityForce =
                (Physics.gravity * _GravityMultiplier) - Physics.gravity;

            _rigidbody.AddForce(extraGravityForce);
        }

        public void HandleGroundedMovement(Vector3 moveDirection, bool shouldApproach)
        {
            Vector3 horizontalVelocity =
                shouldApproach
                    ? moveDirection.normalized * _approachJumpPower
                    : new Vector3(_rigidbody.velocity.x, 0f, _rigidbody.velocity.z);

            _rigidbody.velocity = new Vector3(
                horizontalVelocity.x,
                _jumpPower,
                horizontalVelocity.z
            );

            SetApplyRootMotion(false);
            TriggerJump();
        }

        public void HandleSpinJumpMovement(
            Vector3 moveDirection,
            bool shouldAirApproach)
        {
            Vector3 horizontalVelocity =
                shouldAirApproach
                    ? moveDirection.normalized * _approachSpinJumpPower
                    : Vector3.zero;

            _rigidbody.velocity = new Vector3(
                horizontalVelocity.x,
                _spinJumpPower,
                horizontalVelocity.z
            );
            SetApplyRootMotion(false);
            StartSpinJump();
        }

        public void ApplyExtraTurnRotation()
        {
            float turnSpeed = Mathf.Lerp(
                _stationaryTurnSpeed,
                _movingTurnSpeed,
                Mathf.Abs(_forwardAmount)
            );

            transform.Rotate(0f, _turnAmount * turnSpeed * Time.deltaTime, 0f);
        }

        public void SetApplyRootMotion(bool isRootMotion)
        {
            _animator.applyRootMotion = isRootMotion;
        }

        public void SetGroundState(bool grounded)
        {
            _isGrounded = grounded;
        }

        internal void TriggerJump()
        {
            _animator.SetFloat("Jump", 1f);
        }

        public void StartJump()
        {
            _animator.SetFloat("Jump", 1f);
        }

        public void JumpFinished()
        {
            _onJumpFinished.OnNext(Unit.Default);
        }

        public void StartSpinJump()
        {
            _animator.SetFloat("Jump", 1f);
            _animator.SetTrigger("TriggerSpinJump");
        }

        public void SpinJumpFinished()
        {
            _onSpinJumpFinished.OnNext(Unit.Default);
            _animator.ResetTrigger("TriggerSpinJump");
        }

        public void SetMode(AIMovementMode aIMovementMode)
        {
            if (aIMovementMode == AIMovementMode.NavMesh)
            {
                _animator.applyRootMotion = false;
                _rigidbody.isKinematic = true;
                _agent.enabled = true;
                _agent.updatePosition = false;
                _agent.updateRotation = false;
                _agent.autoBraking = false;

                _agent.nextPosition = transform.position;
            }
            else
            {
                _animator.applyRootMotion = false;

                if (!_isDead) { _rigidbody.isKinematic = false; }
                else { _rigidbody.isKinematic = false; }
            
                 _agent.enabled = false;
                _agent.updatePosition = false;
                _agent.updateRotation = false;
            }
        }

        public void SetDestination(Vector3 target)
        {
            _targetPosition = target;

            if (!_agent.enabled) return;

            _targetPosition = target;
            if (!_agent.enabled) return;

            // Agentに目的地を教える。これだけでAgentは裏側で走り出します。
            _agent.destination = target;
        }

        public Vector3 TransformDirection(Vector3 localDirection)
        {
            return transform.TransformDirection(localDirection);
        }

        public Vector3 Velocity
        {
            get
            {
                if (_agent.enabled)
                {
                    return _agent.desiredVelocity;
                }
                return _rigidbody.velocity;
            }
        }

        public void OnDead()
        {
            if (_isDead)
                return;

            _isDead = true;

            if (_agent.enabled)
            {
                _agent.ResetPath();
                _agent.isStopped = true;
                _agent.enabled = false;
            }

            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.isKinematic = true;
            _animator.ResetTrigger("TriggerSpinJump");

            _animator.applyRootMotion = false;
        }

        private bool _isDead;
        public bool IsDead => _isDead;
    }
}