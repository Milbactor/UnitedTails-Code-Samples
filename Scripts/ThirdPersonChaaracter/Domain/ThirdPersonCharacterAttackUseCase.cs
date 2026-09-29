using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public class ThirdPersonCharacterAttackUseCase : ICharacterAttackUseCase
    {
        private ICharacterStateModel _model;
        private CombatSetting _combatSetting;

        public ThirdPersonCharacterAttackUseCase(ICharacterStateModel thirdPersonMovementModel, CombatSetting combatSetting)
        {
            _model = thirdPersonMovementModel;
           _combatSetting = combatSetting;
        }

        public IReadOnlyReactiveProperty<bool> IsAttacking => _model.IsAttacking;
        public IReadOnlyReactiveProperty<bool> IsSpinAttacking => _model.IsSpinAttacking;
        public IReadOnlyReactiveProperty<bool> IsEquipping => _model.IsEquipping;
        public IReadOnlyReactiveProperty<bool> IsEquipped => _model.IsEquipped;
        public IReadOnlyReactiveProperty<bool> IsUnequipping => _model.IsUnequipping;

        public void Tick()
        {
            if (!_model.IsEquipped.Value) return;

            if (_model.AttackCooldownTimer.Value > 0)
            {
                _model.AttackCooldownTimer.Value -= Time.deltaTime;

                if (_model.AttackCooldownTimer.Value < 0)
                    _model.AttackCooldownTimer.Value = 0;
            }
        }

        public bool CanAttack()
        {
            return !_model.IsAttacking.Value
                && _model.IsEquipped.Value
                && _model.AttackCooldownTimer.Value <= 0f;
        }

        public void OnAttackRequested()
        {
            if (!_model.IsEquipped.Value)
            {
                EquipStarted();
                return;
            }

            if (CanAttack())
            {
                if (_model.IsOnGround.Value == true) { OnAttackStarted(); }
                else
                {
                    if (_model.GroundDistance.Value > _combatSetting.MinGroundDistance)
                    {
                        OnSpinAttackStarted();
                    }
                }
            }
        }

        public void OnAttackStarted()
        {
            _model.AttackCooldownTimer.Value = _combatSetting.AttackCooldownTime;
            _model.IsAttacking.Value = true;
        }

        public void OnAttackFinished()
        {
            if (_model.AttackCooldownTimer.Value < 0) _model.AttackCooldownTimer.Value = 0;
            _model.IsAttacking.Value = false;
        }

        public void OnSpinAttackStarted()
        {
            _model.AttackCooldownTimer.Value = _combatSetting.SpinJumpAttackCoolDownTime;
            _model.IsSpinAttacking.Value = true;
        }

        public void OnSpinAttackFinished()
        {
            if (_model.AttackCooldownTimer.Value < 0) _model.AttackCooldownTimer.Value = 0;
            _model.IsSpinAttacking.Value = false;
        }

        public void EquipStarted()
        {
            if (!CanEquip()) return;
            _model.IsEquipping.Value = true;
        }

        public void OnEquipped()
        {
            _model.IsEquipping.Value = false;
            _model.IsEquipped.Value = true;
        }

        public void OnUnequipRequested()
        {
            if (CanUnequip())
            {
                _model.IsUnequipping.Value = true;
            }
        }

        public void OnUnequipped()
        {
            _model.IsUnequipping.Value = false;
            _model.IsEquipped.Value = false;
        }

        public bool CanEquip()
        {
            return !_model.IsEquipped.Value
                && !_model.IsUnequipping.Value
                && !_model.IsAttacking.Value
                && !_model.IsSpinAttacking.Value;
        }

        public bool CanUnequip()
        {
            return _model.IsEquipped.Value
                && !_model.IsEquipping.Value
                && !_model.IsAttacking.Value
                && !_model.IsSpinAttacking.Value;
        }

        public void ForceGiveUpDeadEnemy()
        {
            _model.IsAttacking.Value = false;
            _model.AttackCooldownTimer.Value = _combatSetting.AttackCooldownTime;
        }
    }
}




