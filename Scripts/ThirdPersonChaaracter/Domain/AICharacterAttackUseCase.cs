using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public class AICharacterAttackUseCase : IAIAttackUseCase, ICharacterLifeCycle
    {
        private readonly IAIBehaviourInput _input;
        private readonly ICharacterStateModel _stateModel;
        private readonly IAIStateModel _aiStateModel;
        public IReadOnlyReactiveProperty<bool> IsAttacking => _stateModel.IsAttacking;
        public IReadOnlyReactiveProperty<bool> IsSpinAttacking => _stateModel.IsSpinAttacking;
        public IReadOnlyReactiveProperty<bool> RequestAttack => _aiStateModel.RequestAttack;
        public IReactiveProperty<bool> RequestSpinAttack => _aiStateModel.RequestSpinAttack;

        private CombatSetting _combatSetting = null;

        public IReadOnlyReactiveProperty<bool> SpinAttackRequestedInThisJump { get; }

        private bool _isDead = false;
        public bool IsDead => _isDead;

        public AICharacterAttackUseCase(
            ICharacterStateModel characterStateModel,
            IAIStateModel aiStateModel,
            CombatSetting combatSetting,
            IAIBehaviourInput input,
            CharacterDeathNotifier characterDeathNotifier)
        {
            _stateModel = characterStateModel;
            _aiStateModel = aiStateModel;
            _combatSetting = combatSetting;
            _input = input;

            characterDeathNotifier.Register(this);

            SpinAttackRequestedInThisJump =
                Observable.CombineLatest(
                    _stateModel.GroundDistance,
                    _stateModel.IsOnGround,
                    _stateModel.IsJumping,
                    _stateModel.IsSpinJumping,
                    (
                        groundDistance,
                        isOnGround,
                        isJumping,
                        isSpinJumping) =>
                        !_isDead &&
                        (
                            _aiStateModel.CurrentMoveState == AIMoveState.ChasingEnemy ||
                            _aiStateModel.CurrentMoveState == AIMoveState.Attacking
                        ) &&
                        _input.HasTarget &&
                        _input.DistanceToTarget <= _input.SpinAttackRange &&
                        groundDistance > 1f &&
                        !isOnGround &&
                        (isJumping || isSpinJumping))
                .DistinctUntilChanged()
                .ToReadOnlyReactiveProperty();
        }

        public void Tick()
        {
            if (_isDead)
                return;

            Timer();

            if (!_input.HasTarget)
                return;

            if (_input.PreferAerialAttack())
            {
                if (!_stateModel.IsOnGround.Value &&
                    _input.DistanceToTarget < _input.SpinAttackRange)
                {
                    OnRequestSpinAttack();
                }
                return;
            }

            if (_input.DistanceToTarget < _input.AttackRange)
            {
                OnRequestAttack();
            }
        }

        private void Timer()
        {
            _stateModel.AttackCooldownTimer.Value =
               Mathf.Max(
               0f,
               _stateModel.AttackCooldownTimer.Value - Time.deltaTime
               );

            _stateModel.SpinAttackCooldownTimer.Value =
                Mathf.Max(
                0f,
                _stateModel.SpinAttackCooldownTimer.Value - Time.deltaTime
                );
        }

        public void OnAttackRequested()
        {
            if (!_stateModel.IsEquipped.Value)
            {
                _stateModel.IsEquipping.Value = true;
                return;
            }
            ExecuteAttack();
        }

        public void OnSpinAttackRequested()
        {
            if (_input == null || !_input.HasTarget)
                return;

            ExecuteSpinAttack();
        }
        
        private void ExecuteAttack()
        {
            if (CanAttack())
            {
                OnAttackStarted();
                return;
            }
            else { _aiStateModel.RequestAttack.Value = false; }

            if (CanSpinAttack())
            {
                OnSpinAttackStarted();
            }
            else  { _aiStateModel.RequestSpinAttack.Value = false; }
        }

        private void ExecuteSpinAttack()
        {
            if (!CanSpinAttack())
            {
                _aiStateModel.RequestSpinAttack.Value = false;
                return;
            }
            OnSpinAttackStarted();
        }

        private bool CanAttack()
        {
            bool result =
                !_stateModel.IsAttacking.Value
                && _stateModel.IsEquipped.Value
                && _stateModel.AttackCooldownTimer.Value <= 0f
                && _stateModel.IsOnGround.Value;

            return result;
        }

        private bool CanSpinAttack()
        {
            bool result =
             !_stateModel.IsSpinAttacking.Value
             && _stateModel.IsEquipped.Value
             && _stateModel.SpinAttackCooldownTimer.Value <= 0f
             && !_stateModel.IsOnGround.Value
             && _stateModel.GroundDistance.Value > _combatSetting.MinGroundDistance;
            return result;
        }

        public void OnAttackStarted()
        {
            _stateModel.AttackCooldownTimer.Value = _combatSetting.AttackCooldownTime;
            _stateModel.IsAttacking.Value = true;
        }

        public void OnAttackFinished() 
        {
            _stateModel.IsAttacking.Value = false;
            _aiStateModel.RequestAttack.Value = false;
        }

        public void OnSpinAttackStarted()
        {
            _stateModel.SpinAttackCooldownTimer.Value = _combatSetting.SpinJumpAttackCoolDownTime;
            _stateModel.IsSpinAttacking.Value = true;
        }

        public void OnSpinAttackFinished()
        {
            _stateModel.IsSpinAttacking.Value = false;
            _aiStateModel.RequestSpinAttack.Value = false;
        }

        public void ForceGiveUpDeadEnemy()
        {
            OnAttackFinished();
            OnSpinAttackFinished();
        }

        public void OnRequestAttack()
        {
            if (_aiStateModel.RequestAttack.Value == true) return;
            _aiStateModel.RequestAttack.Value = true;
        }

        public void OnRequestSpinAttack()
        {
            if (_aiStateModel.RequestSpinAttack.Value == true) return;
            _aiStateModel.RequestSpinAttack.Value = true;
        }

        public void OnDead()
        {
            if (_isDead)
                return;

            _isDead = true;

            _aiStateModel.RequestAttack.Value = false;
            _aiStateModel.RequestSpinAttack.Value = false;
            _aiStateModel.RequestUnequip.Value = false;
            _aiStateModel.RequestEquip.Value = false;
            _aiStateModel.RequestSpinJump.Value = false;

            _stateModel.IsAttacking.Value = false;
            _stateModel.IsSpinAttacking.Value = false;
        }
    }
}