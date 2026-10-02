using System;
using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public class AICharacterDamageUseCase : IThirdPeresonCharacterDamageUseCase, ICharacterLifeCycle
    {
        private readonly ICharacterStateModel _model;
        private readonly CharacterDeathNotifier _characterDeathNotifier;
        private readonly CombatSetting _combatSetting;

        private const float DamageCooldownSeconds = 0.85f;

        private IDisposable _damageCooldownDisposable;

        public IReadOnlyReactiveProperty<bool> IsDead => _model.IsDead;
        public IReadOnlyReactiveProperty<float> HP => _model.HP;
        bool ICharacterLifeCycle.IsDead => _model.IsDead.Value;

        public AICharacterDamageUseCase(
            ICharacterStateModel movementModel,
            CombatSetting combatSetting,
            CharacterDeathNotifier characterDeathNotifier)
        {
            _combatSetting = combatSetting;
            _model = movementModel;

            _characterDeathNotifier = characterDeathNotifier;
            _characterDeathNotifier.Register(this);

            _model.HP.Value = _combatSetting.MaxHP;
            _model.DamageCoolDownTimer.Value = 0f;
        }

        public void TakeDamage(float damage)
        {
            if (!CanTakeDamage())
                return;

            _model.HP.Value =
                Mathf.Max(0f, _model.HP.Value - damage);

            if (_model.HP.Value <= 0f)
            {
                _model.IsDead.Value = true;
                return;
            }

            StartDamageCooldown();
        }

        public bool CanTakeDamage()
        {
            return !_model.IsDead.Value &&
                   _model.HP.Value > 0f &&
                   _model.DamageCoolDownTimer.Value <= 0f;
        }

        private void StartDamageCooldown()
        {
            _damageCooldownDisposable?.Dispose();

            _model.DamageCoolDownTimer.Value =
                DamageCooldownSeconds;

            _damageCooldownDisposable =
                Observable.EveryUpdate()
                    .TakeWhile(_ =>
                        _model.DamageCoolDownTimer.Value > 0f)
                    .Subscribe(
                        _ =>
                        {
                            _model.DamageCoolDownTimer.Value =
                                Mathf.Max(
                                    0f,
                                    _model.DamageCoolDownTimer.Value -
                                    Time.deltaTime);
                        },
                        () =>
                        {
                            _model.DamageCoolDownTimer.Value = 0f;
                            _damageCooldownDisposable = null;
                        });
        }

        public void OnDead()
        {
            _damageCooldownDisposable?.Dispose();
            _damageCooldownDisposable = null;
            _model.DamageCoolDownTimer.Value = 0f;

            _model.IsJumping.Value = false;
            _model.IsSpinJumping.Value = false;
            _model.ShouldApplyRootMotion.Value = false;
            _model.IsSpinAttacking.Value = false;
            _model.IsEquipping.Value = false;
            _model.IsUnequipping.Value = false;
            _model.IsEquipped.Value = false;
            _model.AttackCooldownTimer.Value = 0f;
            _model.SpinAttackCooldownTimer.Value = 0f;
            _model.IsAttacking.Value = false;
        }

        public void Dispose()
        {
            _damageCooldownDisposable?.Dispose();
            _damageCooldownDisposable = null;
        }
    }
}