using UniRx;

namespace WhiteKNight
{
    public class HPUseCase : IHPUseCase
    {
        private CombatSetting _combatSetting;

        private ReactiveProperty<float> _currentHP = new ReactiveProperty<float>(0);
        public  IReactiveProperty<float> CurrentHP => _currentHP;

        public HPUseCase(CombatSetting combatSetting)
        {
            _combatSetting = combatSetting;
            _currentHP.Value = _combatSetting.MaxHP;
        }
    }
}