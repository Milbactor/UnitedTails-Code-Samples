using System;
using UniRx;
using UnityEngine;
using Zenject;

namespace WhiteKNight
{
    public class AICharacterAttackView : CombatSettingViewBase, IThirdPersonCharacterAttackView, IEnemyDeathListener, ICharacterLifeCycle
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private GameObject _shoulderSword;
        [SerializeField] private GameObject _handSword;
        [SerializeField] private Collider _swordCollider;
        [SerializeField] private GameObject _slayVFX;
        [SerializeField] private GameObject _slayPoly;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private GameObject _spinVFX;

        [SerializeField] private float _slayVfxDuration = 2f;
        private const float upPower = 10f;
        private const float bounceRecoverTime = 0.3f;

        [SerializeField] private LayerMask _overheadObstacleLayer;

        [SerializeField] private float _overheadCheckRadius = 0.45f;
        [SerializeField] private float _overheadCheckDistance = 2f;

        [SerializeField] private float _escapeBackPower = 10f;
        [SerializeField] private float _escapeUpPower = 2f;

        private IDisposable _bounceRecoverDisposable;

        public IObservable<Unit> OnUpdate =>
        Observable.EveryUpdate().AsUnitObservable();

        private readonly Subject<Unit> _onAttackHitFinished = new Subject<Unit>();
        public IObservable<Unit> OnAttackHitFinished => _onAttackHitFinished;

        private readonly Subject<Unit> _onEquipFinished = new Subject<Unit>();
        public IObservable<Unit> OnEquipFinished => _onEquipFinished;

        private readonly Subject<Unit> _onUnequipFinished = new Subject<Unit>();
        public IObservable<Unit> OnUnequipFinished => _onUnequipFinished;

        private readonly Subject<Unit> _onSpinAttackFinished = new Subject<Unit>();
        public IObservable<Unit> OnSpinAttackFinished => _onSpinAttackFinished;

        private bool _isDead = false;
        public bool IsDead => _isDead;

        private readonly CompositeDisposable _vfxDisposables = new();


        [Inject]
        public void Construct(CharacterDeathNotifier characterDeathNotifier)
        {
            characterDeathNotifier.Register(this);
        }

        protected override void Awake()
        {
            base.Awake();
            _shoulderSword.SetActive(true);
            _handSword.SetActive(false);
            _slayPoly.SetActive(false);
            _swordCollider.enabled = false;
            _slayVFX.SetActive(false);
            _spinVFX.SetActive(false);
        }

        public void Equip()
        {
            _shoulderSword.SetActive(false);
            _handSword.SetActive(true);

            _animator.SetBool("IsEquipped", true);
        }

        public void OnEquipEnded()
        {
            _onEquipFinished.OnNext(Unit.Default);
        }

        public void Unequip()
        {
            _animator.SetBool("IsEquipped", false);
        }

        public void OnUnequipEnded()
        {
            ResetAttackTriggers();
     
            _handSword.SetActive(false);
            _shoulderSword.SetActive(true);
            _onUnequipFinished.OnNext(Unit.Default);
        }

        public void Attack()
        {
            _animator.SetTrigger("TriggerAttack");
            OnAttackHitStart();
        }

        public void OnAttackHitStart()
        {

            _slayVFX.SetActive(true);
            _vfxDisposables.Clear();
            _slayPoly.SetActive(true);
            _swordCollider.enabled = true;

            Observable.Timer(TimeSpan.FromSeconds(_slayVfxDuration))
                .Subscribe(_ => OnAttackSlayFinished())
                .AddTo(_vfxDisposables);
        }

        private void OnAttackSlayFinished()
        {
            _slayVFX.SetActive(false);
            _slayPoly.SetActive(false);
            _swordCollider.enabled = false;
            _onAttackHitFinished.OnNext(Unit.Default);
        }

        public void OnAttackHitEnd()
        {
         //   Debug.Log("AI AttackView: Attack ended");
        }

        public void StartSpinAttack()
        {
            _spinVFX.SetActive(true);
            _swordCollider.enabled = true;
            _animator.SetBool("IsSpinAttack", true);
        }

        public void EndSpinAttack()
        {
            _onSpinAttackFinished.OnNext(Unit.Default);
            _spinVFX.SetActive(false);
            _swordCollider.enabled = false;
            _animator.SetBool("IsSpinAttack", false);
        }

        public void ResetAttackTriggers()
        {
            _animator.SetLayerWeight(1, 0);
            _animator.ResetTrigger("TriggerAttack");
        }

        public void AttackStepIn(Vector3 aimPosition)
        {
            Vector3 start = _rigidbody.position;
            Vector3 direction = aimPosition - start;
            if (direction.sqrMagnitude < 0.01f) return;

            // for the direction of the enemy
            Vector3 moveDirection = (aimPosition - start).normalized;

            // rotation only in horizontal
            Vector3 rotateDirection = moveDirection;
            rotateDirection.y = 0f;

            if (rotateDirection.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(rotateDirection);
                _rigidbody.MoveRotation(targetRotation);
            }

            // direction only toward up
            _rigidbody.AddForce(
                moveDirection * CombatSetting.AttackStepForce,
                ForceMode.Impulse);
        }

        public void Bounce()
        {
            if (_isDead)
                return;

            _rigidbody.isKinematic = false;

            Vector3 bounceVelocity =
                ResolveBounceVelocity();

            _rigidbody.velocity = bounceVelocity;

            _bounceRecoverDisposable?.Dispose();

            _bounceRecoverDisposable =
                Observable.Timer(
                        TimeSpan.FromSeconds(bounceRecoverTime))
                    .Subscribe(_ =>
                    {
                        _rigidbody.velocity = Vector3.zero;
                        _rigidbody.angularVelocity = Vector3.zero;
                        _rigidbody.isKinematic = true;

                        _bounceRecoverDisposable = null;
                    });
        }
        private Vector3 ResolveBounceVelocity()
        {
            Vector3 castOrigin =
                _rigidbody.worldCenterOfMass;

            bool hasObstacleAbove =
                Physics.SphereCast(
                    castOrigin,
                    _overheadCheckRadius,
                    Vector3.up,
                    out RaycastHit hit,
                    _overheadCheckDistance,
                    _overheadObstacleLayer,
                    QueryTriggerInteraction.Ignore);

            if (!hasObstacleAbove)
            {
                return Vector3.up * upPower;
            }

            Vector3 awayDirection =
                _rigidbody.position -
                hit.collider.bounds.center;

            awayDirection.y = 0f;

            // Ballの真下にいて水平方向を算出できない場合
            if (awayDirection.sqrMagnitude < 0.01f)
            {
                awayDirection =
                    -_rigidbody.transform.forward;
            }

            awayDirection.Normalize();

            return
                awayDirection * _escapeBackPower +
                Vector3.up * _escapeUpPower;
        }

        public void OnEnemyDied(IEnemyView enemy)
        {
            OnAttackSlayFinished();
            EndSpinAttack();
        }

        public void OnDead()
        {
            _bounceRecoverDisposable?.Dispose();
            _bounceRecoverDisposable = null;
            OnAttackSlayFinished();
            EndSpinAttack();
        }
    }
}
