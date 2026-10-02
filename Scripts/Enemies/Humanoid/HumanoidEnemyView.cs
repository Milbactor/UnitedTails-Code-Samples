using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AI;
using UniRx;
using UnityEngine;
using Color = UnityEngine.Color;
using System.Linq.Expressions;

namespace WhiteKNight
{
    public class HumanoidEnemyView : MonoBehaviour, IEnemyView
    {
        [Header("Moving area")]
        [SerializeField] private float _moveRadius = 8f;

        [Header("walk velocity")]
        [SerializeField] private float _walkSpeed = 1.5f;

        [Header("Run velocity")]
        [SerializeField] private float _runSpeed = 1.5f;

        [Header("Interval to update destination")]
        [SerializeField] private float _moveInterval = 3f;
        public float MoveInterval => _moveInterval;

        [Header("Ground Layer")]
        [SerializeField] private LayerMask _groundLayer;

        [SerializeField] private NavMeshAgent _navMeshAgent;

        [SerializeField] private Renderer _renderer;
        private MaterialPropertyBlock _block;

        [Header("bodies' colliders")]
        [SerializeField] private List<Collider> _colliders;

        [SerializeField] private Animator _animator;

        [Header("attack hitboxes")]
        [SerializeField] private List<Collider> _attackHitBoxes;

        [SerializeField] private HitEffectPool _hitEffectPool;

        [SerializeField] private SoundEffectId _damageSoundId;
        [SerializeField] private SoundEffectId _explosionSoundId;
        [SerializeField] private SoundEffectId _screamSoundId;
        [SerializeField] private SoundEffectId _walkSoundId;
        [SerializeField] private SoundEffectId _enemyDeadSoundId;

        private Vector3 originPosition;
        private Vector3 destination;

        public IObservable<Unit> OnUpdate = Observable.EveryUpdate().AsUnitObservable();

        private readonly Subject<DamageGivenInfo> _onDamageTaken = new Subject<DamageGivenInfo>();
        public IObservable<DamageGivenInfo> OnDamageTaken => _onDamageTaken;

        private readonly Subject<Unit> _onAttackFinished = new Subject<Unit>();
        public IObservable<Unit> OnAttackFinished => _onAttackFinished;

        public float AttackRange => _attackRange;
        [SerializeField] private float _attackRange = 10f;

        private bool _isDead;
        public bool IsDead => _isDead;

        public float TargetDistance
        {
            get
            {
                if (CurrentTarget == null)
                    return float.MaxValue;

                return Vector3.Distance(
                    transform.position,
                    CurrentTarget.transform.position);
            }
        }

        private readonly List<Transform> _targets = new();
        public Transform CurrentTarget
        {
            get
            {
                _targets.RemoveAll(target => target == null);

                return _targets
                    .OrderBy(target =>
                        Vector3.Distance(transform.position, target.position))
                    .FirstOrDefault();
            }
        }

        [Header("Attack Cooldown")]
        [SerializeField] private float _attackCooldown = 1.5f;
        public float AttackCooldown => _attackCooldown;

        [Header("Damage Cooldown")]
        [SerializeField] private float _damageCooldown = 1.0f;
        public float DamageCooldown => _damageCooldown;

        private const float DescentWaitingTime = 2.5f;

        [SerializeField] Collider _collider;
        public Collider CombatCollider => _collider;

        [SerializeField] private float _power = 10f;
        public float Power => _power;

        private void Start()
        {
            originPosition = transform.position;
            destination = transform.position;

            _navMeshAgent.speed = _walkSpeed;

            DecideNewDestination();

            if (_renderer == null)
                _renderer = GetComponentInChildren<Renderer>();

            _block = new MaterialPropertyBlock();
        }

        //moving toward target
        public void ChaseCurrentTarget()
        {
            Transform target = CurrentTarget;

            if (target == null || !_navMeshAgent.isOnNavMesh)
                return;
            _navMeshAgent.speed = _runSpeed;
            _navMeshAgent.isStopped = false;
            _navMeshAgent.SetDestination(target.position);
        }

        // moving without target
        public void MoveToDestination()
        {
            if (!_navMeshAgent.isOnNavMesh)
                return;
            _navMeshAgent.speed = _walkSpeed;
            _navMeshAgent.isStopped = false;
            _navMeshAgent.SetDestination(destination);
        }

        public void DecideNewDestination()
        {
            Vector3 randomPosition =
                originPosition +
                UnityEngine.Random.insideUnitSphere * _moveRadius;

            randomPosition.y = originPosition.y;

            if (NavMesh.SamplePosition(
                    randomPosition,
                    out NavMeshHit hit,
                    _moveRadius,
                    NavMesh.AllAreas))
            {
                destination = hit.position;
            }
        }

        public void StopMoving()
        {
            if (!_navMeshAgent.isOnNavMesh)
                return;

            _navMeshAgent.isStopped = true;
            _navMeshAgent.ResetPath();
        }

        public void OnWalkStarted()
        {
            SoundManager.Instance.PlaySE(_walkSoundId);
            _animator.SetBool("IsWalking", true);
        }

        public void OnWalkEnded()
        {
            _animator.SetBool("IsWalking", false);
        }

        public void PlayAttack()
        {
            SetAttackHitBoxEnable(true);
            SoundManager.Instance.PlaySE(_screamSoundId);
            _animator.SetBool("IsAttacking", true);
            Observable.Timer(TimeSpan.FromSeconds(1.2f))
             .Subscribe(_ => OnPlayAttackFinished())
             .AddTo(this);
        }

        private void OnPlayAttackFinished()
        {
            _animator.SetBool("IsAttacking", false);
            _onAttackFinished.OnNext(Unit.Default);
            SetAttackHitBoxEnable(false);
        }

        private void SetAttackHitBoxEnable(bool enable)
        {
            foreach(var hit in _attackHitBoxes)
            {
                hit.enabled = enable;
            }
        }

        public void FaceTarget(Transform target)
        {
            if (target == null)
                return;

            Vector3 direction = target.position - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude < 0.001f)
                return;

            transform.rotation = Quaternion.LookRotation(direction);
        }

        public void ShowGiveDamageEffect(Vector3 position, Vector3 normal)
        {
            SoundManager.Instance.PlaySE(_explosionSoundId);
            _hitEffectPool.ShowEffect(position, normal);
        }

        public void TakeDamage(DamageGivenInfo damageGivenInfo)
        {
            if (_isDead) return;

            bool canTakeDamage = _colliders.Any(collider => collider.enabled);
            if (!canTakeDamage) return;

            SetCollidersEnabled(false);

            SoundManager.Instance?.PlaySE(_damageSoundId);
            _onDamageTaken.OnNext(damageGivenInfo);
            _animator.SetTrigger("TriggerDamage");
        }

        public void OnTakeDamageStarted(float hpRate)
        {
            SetCollidersEnabled(false);

            int blinkCount = Mathf.Max(
                1,
                Mathf.CeilToInt(_damageCooldown / 0.1f)
            );

            Observable.Interval(
                    TimeSpan.FromSeconds(0.1f))
                .Take(blinkCount)
                .Subscribe(
                    x =>
                    {
                        bool blink = x % 2 == 0;

                        Color targetColor =
                            Color.Lerp(
                                Color.white,
                                Color.black,
                                hpRate
                            );

                        Color finalColor =
                            blink
                                ? Color.white
                                : targetColor;

                        _block.SetColor(
                            "_Color",
                            finalColor
                        );

                        _renderer.SetPropertyBlock(_block);
                    },
                    OnTakeDamageEnded
                )
                .AddTo(this);

            SoundManager.Instance.PlaySE(
                _screamSoundId
            );
        }

        private void OnTakeDamageEnded()
        {
            if (_isDead)
                return;

            SetCollidersEnabled(true);
            _animator.ResetTrigger("TriggerDamage");
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
            if (_isDead) return;
            _isDead = true;
            SetCollidersEnabled(false);
            SoundManager.Instance.PlaySE(_enemyDeadSoundId);
            foreach (Transform t in GetComponentsInChildren<Transform>(true))
            {
                t.gameObject.layer = LayerMask.NameToLayer("Corpse");
                t.gameObject.tag = "Corpse";
            }
            _navMeshAgent.enabled = false;
            EnemyManager.Instance?.OnEnemyDied(this);
            _animator.SetTrigger("TriggerDead");
            StartCoroutine(DeathSequence());
        }

        private IEnumerator DeathSequence()
        {
            yield return new WaitForSeconds(DescentWaitingTime);
            GetComponent<CharacterDepartureView>().StartDeparture();
        }

        public void AddTarget(Transform target)
        {
            if (target == null) return;
            if (!_targets.Contains(target)) { _targets.Add(target); }
        }

        public void RemoveTarget(Transform target)
        {
            if (target == null) return;
            _targets.Remove(target);
        }
    }


}


