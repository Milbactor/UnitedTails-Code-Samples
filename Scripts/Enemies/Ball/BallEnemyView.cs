using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public sealed class BallEnemyView : MonoBehaviour, IEnemyView
    {
        [Header("Moving area")]
        [SerializeField] private float _moveRadius = 8f;

        [Header("Jump velocity")]
        [SerializeField] private float _jumpSpeed = 1.5f;

        [Header("Time attack uses to reach player")]
        [SerializeField] private float _attackReachingTime = 3f;

        [SerializeField] private float _attackUpwardSpeed = 5f;
        [SerializeField] private float _attackMoveSpeed = 8f;
        [SerializeField] private float _attackSteering = 10f;

        private const float HorizontalBouncePower = 2f;
        private const float UpwardBouncePower = 4f;

        [Header("Ground Layer")]
        [SerializeField] private LayerMask _groundLayer;

        [Header("Ray Height")]
        [SerializeField] private float _rayHeight = 10f;

        [Header("Renderer")]
        [SerializeField] private Renderer _renderer;

        [Header("Body colliders")]
        [SerializeField] private List<Collider> _colliders;

        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private HitEffectPool _hitEffectPool;

        [Header("Sound effects")]
        [SerializeField] private SoundEffectId _damageSoundId;
        [SerializeField] private SoundEffectId _explosionSoundId;
        [SerializeField] private SoundEffectId _screamSoundId;
        [SerializeField] private SoundEffectId _walkSoundId;
        [SerializeField] private SoundEffectId _enemyDeadSoundId;

        [Header("Landing check")]
        [SerializeField] private float _arrivalDistance = 0.8f;
        [SerializeField] private float _landingVelocityThreshold = 0.3f;

        private const float DescentWaitingTime = 2.5f;

        private readonly List<Transform> _targets = new List<Transform>();
        private readonly Subject<DamageGivenInfo> _onDamageTaken = new Subject<DamageGivenInfo>();
        private readonly Subject<Unit> _onDamageEnded = new Subject<Unit>();

        private readonly Subject<Unit> _onAttackStarted = new Subject<Unit>();
        private readonly Subject<Unit> _onAttackFinished = new Subject<Unit>();
        private readonly Subject<Unit> _onJumpStarted = new Subject<Unit>();
        private readonly Subject<Unit> _onJumpFinished = new Subject<Unit>();
        private readonly Subject<Unit> _onResetWaitingForNextJump = new Subject<Unit>();

        private readonly Subject<Transform> _onTargetEntered = new Subject<Transform>();
        private readonly Subject<Transform> _onTargetExited = new Subject<Transform>();
        public IObservable<Transform> OnTargetEntered => _onTargetEntered;
        public IObservable<Transform> OnTargetExited => _onTargetExited;

        private MaterialPropertyBlock _propertyBlock;
        private Vector3 _originPosition;
        private bool _isDead;

        public IObservable<Unit> OnUpdate => Observable.EveryUpdate().AsUnitObservable();

        private Subject<Unit> _onDamageStarted = new Subject<Unit>();
        public IObservable<Unit> OnDamageStarted => _onDamageStarted;
        public IObservable<DamageGivenInfo> OnDamageTaken => _onDamageTaken;
        public IObservable<Unit> OnDamageEnded => _onDamageEnded;
        public IObservable<Unit> OnAttackStarted => _onAttackStarted;
        public IObservable<Unit> OnAttackFinished => _onAttackFinished;
        public IObservable<Unit> OnJumpStarted => _onJumpStarted;
        public IObservable<Unit> OnJumpFinished => _onJumpFinished;

        public bool IsDead => _isDead;

        public Transform CurrentTarget =>
            _targets
                .Where(target => target != null)
                .OrderBy(target =>
                    (target.position - transform.position).sqrMagnitude)
                .FirstOrDefault();

        [SerializeField] private Collider _collider;
        public Collider CombatCollider => _collider;

        [SerializeField]
        private OffScreenIndicatorView _offScreenIndicatorView;

        private void Awake()
        {
            _originPosition = transform.position;

            if (_renderer == null)
                _renderer = GetComponentInChildren<Renderer>();

            _propertyBlock = new MaterialPropertyBlock();
        }

        public void StartAttack()
        {
            if (_isDead)
                return;

            if (!TryStartJumpToAttack())
            {
                FinishAttack();
                return;
            }

            SoundManager.Instance?.PlaySE(_screamSoundId);

            _onAttackStarted.OnNext(Unit.Default);

            Observable.Timer(TimeSpan.FromSeconds(_attackReachingTime))
                .Subscribe(_ => FinishAttack())
                .AddTo(this);
        }

        private bool TryStartJumpToAttack()
        {
            if (_isDead)
                return false;
            Transform target = CurrentTarget;
            if (target == null)
                return false;

            float reachingTime = Mathf.Max(_attackReachingTime, 0.01f);

            Vector3 toTarget = target.position - transform.position;

            Vector3 desiredVelocity = toTarget / reachingTime;

            desiredVelocity.y = _attackUpwardSpeed;

            Vector3 velocityChange = desiredVelocity - _rigidbody.velocity;

            _rigidbody.AddForce(
                velocityChange,
                ForceMode.VelocityChange);

            _rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);

            _onJumpStarted.OnNext(Unit.Default);
            return true;
        }

        public void SteerAttackTowardTarget()
        {
            Transform target = CurrentTarget;
            if (target == null)
                return;

            Vector3 toTarget =
                target.position - _rigidbody.position;

            toTarget.y = 0f;

            if (toTarget.sqrMagnitude < 0.0001f)
                return;

            Vector3 desiredHorizontalVelocity =
                toTarget.normalized * _attackMoveSpeed;

            Vector3 currentVelocity =
                _rigidbody.velocity;

            Vector3 currentHorizontalVelocity =
                new Vector3(
                    currentVelocity.x,
                    0f,
                    currentVelocity.z);

            Vector3 newHorizontalVelocity =
                Vector3.MoveTowards(
                    currentHorizontalVelocity,
                    desiredHorizontalVelocity,
                    _attackSteering * Time.fixedDeltaTime);

            _rigidbody.velocity = new Vector3(
                newHorizontalVelocity.x,
                currentVelocity.y,
                newHorizontalVelocity.z);
        }

        private void FinishAttack()
        {
            if (_isDead)
                return;

            Vector3 velocity = _rigidbody.velocity;
            velocity.x = 0f;
            velocity.z = 0f;
            _rigidbody.velocity = velocity;

            _onAttackFinished.OnNext(Unit.Default);
        }

        public void StartRandomJump()
        {
            if (_isDead)
                return;

            Vector3 destination = DecideRandomDestination();

            Vector3 horizontalDirection = destination - transform.position;
            horizontalDirection.y = 0f;

            if (horizontalDirection.sqrMagnitude < 0.0001f)
                return;

            horizontalDirection.Normalize();

            Vector3 jumpDirection =
                horizontalDirection +
                Vector3.up * 0.3f;

            jumpDirection.Normalize();

            _rigidbody.AddForce(
                jumpDirection * _jumpSpeed,
                ForceMode.VelocityChange);

            SoundManager.Instance?.PlaySE(_walkSoundId);
            _onJumpStarted.OnNext(Unit.Default);
        }

        private Vector3 DecideRandomDestination()
        {
            Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * _moveRadius;
            Vector3 desiredPosition = _originPosition + new Vector3(
                randomCircle.x,
                0f,
                randomCircle.y);

            return ResolveGroundPosition(desiredPosition);
        }

        private Vector3 ResolveGroundPosition(Vector3 desiredPosition)
        {
            Vector3 rayStart = desiredPosition + Vector3.up * _rayHeight;

            if (Physics.Raycast(
                    rayStart,
                    Vector3.down,
                    out RaycastHit hit,
                    50f,
                    _groundLayer))
            {
                return hit.point;
            }

            return desiredPosition;
        }

        public void CheckLanding()
        {
            if (_rigidbody.velocity.y > 0f)
                return;

            Vector3 rayStart = transform.position + Vector3.up * 0.1f;

            bool isNearGround = Physics.Raycast(
                rayStart,
                Vector3.down,
                out _,
                _arrivalDistance,
                _groundLayer);

            if (!isNearGround)
                return;

            if (Mathf.Abs(_rigidbody.velocity.y) > _landingVelocityThreshold)
                return;

            FinishJump();
        }

        private void FinishJump()
        {
            _onJumpFinished.OnNext(Unit.Default);
        }

        public void TakeDamage(DamageGivenInfo damageGivenInfo)
        {
            if (_isDead)
                return;

            SetCollidersEnabled(false);
            SoundManager.Instance?.PlaySE(_damageSoundId);
            _onDamageTaken.OnNext(damageGivenInfo);
        }

        public void OnTakeDamageStarted(float hpRate)
        {
            _rigidbody.isKinematic = true;
            SetCollidersEnabled(false);
            _onDamageStarted.OnNext(Unit.Default);

            Observable.Interval(TimeSpan.FromSeconds(0.1f))
                .Take(10)
                .Subscribe(
                    count => UpdateDamageBlink(count, hpRate),
                    OnDamageFinished)
                .AddTo(this);

            SoundManager.Instance?.PlaySE(_screamSoundId);
        }

        public void KnockBack(Vector3 attackerPosition, float maxBounceSpeed = 0)
        {
            if (_isDead)
                return;

            _rigidbody.isKinematic = false;

            Vector3 away = _rigidbody.worldCenterOfMass - attackerPosition;
            away.y = 0f;

            if (away.sqrMagnitude < 0.01f)
                away = transform.forward;

            away.Normalize();
            Vector3 bounceVelocity = away * HorizontalBouncePower + Vector3.up * UpwardBouncePower;

            // 0以下なら制限なし
            if (maxBounceSpeed > 0f)
            {
                bounceVelocity = Vector3.ClampMagnitude(
                    bounceVelocity,
                    maxBounceSpeed);
            }

            _rigidbody.velocity = bounceVelocity;
        }

        private void UpdateDamageBlink(long count, float hpRate)
        {
            bool blinkWhite = count % 2 == 0;
            Color damagedColor = Color.Lerp(Color.white, Color.black, hpRate);
            Color displayColor = blinkWhite ? Color.white : damagedColor;

            _propertyBlock.SetColor("_Color", displayColor);
            _renderer.SetPropertyBlock(_propertyBlock);
        }

        private void OnDamageFinished()
        {
            if (_isDead)
                return;

            _rigidbody.isKinematic = false;
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;

            SetCollidersEnabled(true);
            _onDamageEnded.OnNext(Unit.Default);
        }

        private void SetCollidersEnabled(bool enabled)
        {
            foreach (Collider bodyCollider in _colliders)
            {
                if (bodyCollider != null)
                    bodyCollider.enabled = enabled;
            }
        }

        public void Die()
        {
            if (_isDead)
                return;
            _isDead = true;

            SoundManager.Instance?.PlaySE(_enemyDeadSoundId);

            foreach (Transform child in GetComponentsInChildren<Transform>(true))
            {
                child.gameObject.layer = LayerMask.NameToLayer("Corpse");
                child.gameObject.tag = "Corpse";
            }

            EnemyManager.Instance?.OnEnemyDied(this);
            _rigidbody.isKinematic = true;
            StartCoroutine(DeathSequence());
        }

        private IEnumerator DeathSequence()
        {
            yield return new WaitForSeconds(DescentWaitingTime);
            GetComponent<CharacterDepartureView>().StartDeparture();
        }

        public void ShowGiveDamageEffect(Vector3 position, Vector3 normal)
        {
            SoundManager.Instance?.PlaySE(_explosionSoundId);
            _hitEffectPool.ShowEffect(position, normal);
        }

        public void AddTarget(Transform target)
        {
            if (target != null && !_targets.Contains(target))
                _targets.Add(target);
        }

        public void RemoveTarget(Transform target)
        {
            if (target != null)
                _targets.Remove(target);
        }

        public void OnAttackAreaEntered(Transform playerTransform)
        {
            _onTargetEntered.OnNext(playerTransform);
        }

        public void OnAttackAreaExited(Transform playerTransform)
        {
            _onTargetExited.OnNext(playerTransform);
        }

        private void OnDestroy()
        {
            _onTargetEntered.Dispose();
            _onTargetExited.Dispose();
            _onDamageTaken.Dispose();
            _onDamageEnded.Dispose();
            _onAttackStarted.Dispose();
            _onAttackFinished.Dispose();
            _onJumpStarted.Dispose();
            _onJumpFinished.Dispose();
            _onResetWaitingForNextJump.Dispose();
        }
    }
}