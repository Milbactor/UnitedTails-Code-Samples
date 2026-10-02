using UniRx;

namespace WhiteKNight
{
    public class ThirdPersonCharacterDamageUseCase : IThirdPeresonCharacterDamageUseCase
    {
        private readonly ICharacterStateModel _model;
        public IReadOnlyReactiveProperty<bool> IsDead => _model.IsDead;
        public IReadOnlyReactiveProperty<float> HP => _model.HP;

        private CombatSetting _combatSetting;

        public ThirdPersonCharacterDamageUseCase(ICharacterStateModel movementModel, CombatSetting combatSetting)
        {
            _model = movementModel;
            _combatSetting = combatSetting;
            _model.HP.Value = _combatSetting.MaxHP;
        }

        public void TakeDamage(float damage)
        {
            _model.HP.Value -= damage;
            if (_model.HP.Value <= 0)
            {
                _model.IsDead.Value = true;
            }
        }
    }
}

