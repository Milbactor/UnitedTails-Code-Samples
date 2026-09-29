using System;
using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public partial class ThirdPersonCharacterAttackView : CombatSettingViewBase, IThirdPersonCharacterAttackView
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private GameObject _shoulderSword;
        [SerializeField] private GameObject _handSword;
        [SerializeField] private Collider _swordCollider;
        [SerializeField] private GameObject _slayPoly;
        [SerializeField] private GameObject _slayVFX;
        [SerializeField] private GameObject _spinAttackTragectory;
        private const float _upPower = 10f;
        private const float bounceRecoverTime = 0.3f;
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

        [SerializeField] private CombatSetting _combatSetting;
        private readonly float[] _attackValues =
        {
            0f,
            0.4f,
            0.7f,
            1f
        };

        protected override void Awake()
        {
            base.Awake();
            _shoulderSword.SetActive(true);
            _handSword.SetActive(false);
            _swordCollider.enabled = false;
            _slayVFX.SetActive(false);
            _slayPoly.SetActive(false);
            _spinAttackTragectory.SetActive(false);
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
            _handSword.SetActive(false);
            _shoulderSword.SetActive(true);
            _onUnequipFinished.OnNext(Unit.Default);
        }

        public void Attack()
        {
            if(_animator.GetFloat("Forward") > 0f)
            {
                var equipLayer = _animator.GetLayerIndex("Equip Layer");
                _animator.SetLayerWeight(equipLayer, 0f);
                _animator.SetTrigger("TriggerAttack");
            }
            else
            {
                var equipLayer = _animator.GetLayerIndex("Equip Layer");
                _animator.SetLayerWeight(equipLayer, 1f);
                _animator.SetTrigger("TriggerAttack");
            }
        }

        public void OnAttackHitStart()
        {
            _slayPoly.SetActive(true);
            _swordCollider.enabled = true;
            _slayVFX.SetActive(true);
        }

        public void OnAttackHitEnd()
        {
            _slayVFX.SetActive(false);
            _slayPoly.SetActive(false);
            _swordCollider.enabled = false;
            _onAttackHitFinished.OnNext(Unit.Default);
            var equipLayer = _animator.GetLayerIndex("Equip Layer");
            _animator.SetLayerWeight(equipLayer, 1f);
        }

        public void StartSpinAttack()
        {
            _spinAttackTragectory.SetActive(true);
            _animator.SetBool("IsSpinAttack", true);
        }

        public void EndSpinAttack()
        {
            _onSpinAttackFinished.OnNext(Unit.Default);
            _spinAttackTragectory.SetActive(false);
            _animator.SetBool("IsSpinAttack", false);
        }

        public void AttackStepIn(Vector3 aimPosition)
        {
            Vector3 start = _rigidbody.position;
            Vector3 direction = aimPosition - start;
            if (direction.sqrMagnitude < 0.01f) return;

            // toward enemy
            Vector3 moveDirection = (aimPosition - start).normalized;

            // horizontal rotation only
            Vector3 rotateDirection = moveDirection;
            rotateDirection.y = 0f;

            if (rotateDirection.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(rotateDirection);
                _rigidbody.MoveRotation(targetRotation);
            }

            // movement has height as well
            _rigidbody.AddForce(
                moveDirection * CombatSetting.AttackStepForce,
                ForceMode.Impulse);
        }
        
        public void Bounce()
        {
            Vector3 bounceVelocity =
                 Vector3.up * _upPower;

            _rigidbody.velocity = bounceVelocity;
            Observable.Timer(
                TimeSpan.FromSeconds(bounceRecoverTime))
                .Subscribe(_ =>
                {
                    _rigidbody.velocity = Vector3.zero;
                    _rigidbody.angularVelocity = Vector3.zero;
                })
                .AddTo(this);
        }

        public void Bounce(Vector3 bounceDir)
        {
            _rigidbody.velocity = bounceDir *_upPower;
            Observable.Timer(
                TimeSpan.FromSeconds(bounceRecoverTime))
                .Subscribe(_ =>
                {
                    _rigidbody.velocity = Vector3.zero;
                    _rigidbody.angularVelocity = Vector3.zero;
                })
                .AddTo(this);
        }
    }
}

