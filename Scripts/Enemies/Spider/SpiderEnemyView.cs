using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using Color = UnityEngine.Color;

namespace WhiteKNight
{
    public interface ISpiderEnemyView :IEnemyView
    { 
        float TargetDistance { get; }
    }

    public class SpiderEnemyView : MonoBehaviour, ISpiderEnemyView
    {
        [Header("Moving area")]
        [SerializeField] private float _moveRadius = 8f;

        [Header("Moving velocity")]
        [SerializeField] private float _moveSpeed = 1.5f;

        [Header("Interval to update destination")]
        [SerializeField] private float _moveInterval = 3f;
        public float MoveInterval => _moveInterval;

        [Header("Ground Layer")]
        [SerializeField] private LayerMask _groundLayer;

        [Header("Ray Height")]
        [SerializeField] private float _rayHeight = 10f;

        [SerializeField] private Renderer _renderer;
        private MaterialPropertyBlock _block;

        [Header("bodies' hitbox colliders")]
        [SerializeField] private List<Collider> _colliders;

        [SerializeField] private Animator _animator;

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

        private const float DescentWaitingTime = 1.5f;

        [SerializeField] Collider _collider;
        public Collider CombatCollider => _collider;

        [SerializeField] private float _power = 10f;
        public float Power => _power;

        private void Start()
        {
            originPosition = transform.position;
            destination = transform.position;

            DecideNewDestination();

            if (_renderer == null) { _renderer = GetComponentInChildren<Renderer>(); }
            _block = new MaterialPropertyBlock();
        }

        public void ChaseCurrentTarget()
        {
            if (CurrentTarget == null) return;

            Vector3 targetPosition = new Vector3(
                CurrentTarget.position.x,
                CurrentTarget.position.y,
                CurrentTarget.position.z
            );

            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                _moveSpeed * Time.deltaTime
            );
        }

        public void MoveToDestination()
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                destination,
                _moveSpeed * Time.deltaTime
            );
        }

        public void ShowGiveDamageEffect(Vector3 position, Vector3 normal)
        {
            SoundManager.Instance.PlaySE(_explosionSoundId);
            _hitEffectPool.ShowEffect(position, normal);
        }

        public void OnWalkStarted()
        {
            //SoundManager.Instance.PlaySE(_walkSoundId);
            _animator.SetBool("IsWalking", true);
        }

        public void OnWalkEnded()
        {
            _animator.SetBool("IsWalking", false);
        }

        public void PlayAttack()
        {
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
        }

        public void TakeDamage(DamageGivenInfo damageGivenInfo)
        {
            if (_isDead) return;

            bool canTakeDamage = _colliders.Any(collider => collider.enabled);
            if (!canTakeDamage) return;
            
            SetCollidersEnabled(false);

            SoundManager.Instance?.PlaySE(_damageSoundId);
            _onDamageTaken.OnNext(damageGivenInfo);
        }

        public void OnTakeDamageStarted(float hpRate)
        {
            SetCollidersEnabled(false);
            Observable.Interval(TimeSpan.FromSeconds(0.1f))
                .Take(10)
                .Subscribe(x =>
                {
                    bool blink = x % 2 == 0;

                    Color targetColor = Color.Lerp(Color.white, Color.black, hpRate);
                    Color finalColor = blink ? Color.white : targetColor;

                    _block.SetColor("_Color", finalColor);
                    _renderer.SetPropertyBlock(_block);
                },
                () =>
                {
                    OnTakeDamageEnded();
                })
                .AddTo(this);
            SoundManager.Instance.PlaySE(_screamSoundId);
        }

        private void OnTakeDamageEnded()
        {
            SetCollidersEnabled(true);
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
            if(_isDead) return;
            _isDead = true;
            SetCollidersEnabled(false);
            CombatCollider.enabled = false;
            SoundManager.Instance.PlaySE(_enemyDeadSoundId);
            foreach (Transform t in GetComponentsInChildren<Transform>(true))
            {
                t.gameObject.layer = LayerMask.NameToLayer("Corpse");
                t.gameObject.tag = "Corpse";
            }
            EnemyManager.Instance?.OnEnemyDied(this);
            _animator.SetTrigger("TriggerDead");         
            StartCoroutine(DeathSequence());
        }

        private IEnumerator DeathSequence()
        {
            // 倒れた瞬間
            yield return new WaitForSeconds(DescentWaitingTime);

            GetComponent<CharacterDepartureView>().StartDeparture();

        }

        public void DecideNewDestination()
        {
            Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * _moveRadius;

            Vector3 rayStart = originPosition + new Vector3(
                randomCircle.x,
                _rayHeight,
                randomCircle.y
            );

            if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, 50f, _groundLayer))
            {
                destination = new Vector3(
                    hit.point.x,
                    hit.point.y,
                    hit.point.z
                );
            }
        }

        public void AddTarget(Transform target)
        {
            if (target == null) return;

            if (!_targets.Contains(target))
            {
                _targets.Add(target);
            }
        }

        public void RemoveTarget(Transform target)
        {
            if (target == null) return;

            _targets.Remove(target);
        }
    }
}


