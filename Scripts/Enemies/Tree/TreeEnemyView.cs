using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public class TreeEnemyView : MonoBehaviour, IEnemyView
    {
        [Header("Renderer")]
        [SerializeField] private Renderer _renderer;
        private MaterialPropertyBlock _propertyBlock;

        [Header("Body colliders")]
        [SerializeField] private List<Collider> _colliders;

        [SerializeField] private HitEffectPool _hitEffectPool;

        [Header("Sound effects")]
        [SerializeField] private SoundEffectId _damageSoundId;
        [SerializeField] private SoundEffectId _explosionSoundId;
        [SerializeField] private SoundEffectId _screamSoundId;
        [SerializeField] private SoundEffectId _walkSoundId;
        [SerializeField] private SoundEffectId _enemyDeadSoundId;

        private readonly Subject<DamageGivenInfo> _onDamageTaken = new Subject<DamageGivenInfo>();
        private readonly Subject<Unit> _onDamageTakenEnded = new Subject<Unit>();
        private readonly Subject<DamageGivenInfo> _onDamageGiven = new Subject<DamageGivenInfo>();

        [SerializeField] private List<TreeEnemyHitBoxView> _hitViews;
        public IReadOnlyList<TreeEnemyHitBoxView> HitViews => _hitViews;

        private bool _isDead;
        public bool IsDead => _isDead;
        public IObservable<DamageGivenInfo> OnDamageGiven => _onDamageGiven;
        public IObservable<DamageGivenInfo> OnDamageTaken => _onDamageTaken;
        public IObservable<Unit> OnDamageTakenEnded => _onDamageTakenEnded;

        [SerializeField] private Collider _collider;
        public Collider CombatCollider => _collider;

        [SerializeField] private float _damage;

        [Header("Target tracking")]
        [SerializeField] private Transform _rotationRoot;
        [SerializeField] private Transform _firstJoint;

        [SerializeField] private float _rootRotationSpeed = 4f;
        [SerializeField] private float _jointRotationSpeed = 6f;

        [SerializeField] private Vector3 _jointLookAtOffset = new Vector3(0f, 1f, 0f);
        [SerializeField] private Vector3 _jointRotationOffset;

        [HideInInspector] public Transform LookTarget;

        private void Awake()
        {
            if (_renderer == null)
                _renderer = GetComponentInChildren<Renderer>();

            _propertyBlock = new MaterialPropertyBlock();
        }

        private void LateUpdate()
        {
            if (LookTarget == null || _isDead)
                return;

            RotateRootTowardsTarget();
            RotateFirstJointTowardsTarget();
        }

        private void RotateRootTowardsTarget()
        {
            if (_rotationRoot == null || LookTarget == null)
                return;

            Vector3 direction =
               _rotationRoot.position - LookTarget.position;

            direction.y = 0f;

            if (direction.sqrMagnitude < 0.0001f)
                return;

            Quaternion targetRotation =
                Quaternion.LookRotation(direction, Vector3.up);

            _rotationRoot.rotation = Quaternion.Slerp(
                _rotationRoot.rotation,
                targetRotation,
                _rootRotationSpeed * Time.deltaTime);
        }

        private void RotateFirstJointTowardsTarget()
        {
            if (_firstJoint == null)
                return;

            Vector3 targetPosition =
                LookTarget.position + _jointLookAtOffset;

            Vector3 direction =
                targetPosition - _firstJoint.position;

            if (direction.sqrMagnitude < 0.0001f)
                return;

            Quaternion lookRotation =
                Quaternion.LookRotation(direction.normalized, Vector3.up);

            Quaternion rotationOffset =
                Quaternion.Euler(_jointRotationOffset);

            Quaternion targetRotation =
                lookRotation * rotationOffset;

            _firstJoint.rotation = Quaternion.Slerp(
                _firstJoint.rotation,
                targetRotation,
                _jointRotationSpeed * Time.deltaTime);
        }

        public void OnTakeDamageStarted(float hpRate)
        {
            SetCollidersEnabled(false);

            Observable.Interval(TimeSpan.FromSeconds(0.1f))
                .Take(10)
                .Subscribe(
                    count => UpdateDamageBlink(count, hpRate),
                    OnTakeDamageFinished)
                .AddTo(this);

            SoundManager.Instance?.PlaySE(_screamSoundId);
        }

        private void UpdateDamageBlink(long count, float hpRate)
        {
            bool blinkWhite = count % 2 == 0;
            Color damagedColor = Color.Lerp(Color.white, Color.black, hpRate);
            Color displayColor = blinkWhite ? Color.white : damagedColor;

            _propertyBlock.SetColor("_Color", displayColor);
            _renderer.SetPropertyBlock(_propertyBlock);
        }

        public void TakeDamage(DamageGivenInfo damageGivenInfo)
        {
            if (_isDead)
                return;

            SetCollidersEnabled(false);
            SoundManager.Instance?.PlaySE(_damageSoundId);
            _onDamageTaken.OnNext(damageGivenInfo);
        }

        private void OnTakeDamageFinished()
        {
            if (_isDead)
                return;

            SetCollidersEnabled(true);
            _onDamageTakenEnded.OnNext(Unit.Default);
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

            foreach(var collider in _colliders)
            {
                collider.isTrigger = false;
            }
            EnemyManager.Instance?.OnEnemyDied(this);
        }

        public void ShowGiveDamageEffect(Vector3 position, Vector3 normal)
        {
            SoundManager.Instance?.PlaySE(_explosionSoundId);
            _hitEffectPool.ShowEffect(position, normal);
        }
    }
}