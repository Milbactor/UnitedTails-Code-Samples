using UniRx;

namespace WhiteKNight
{
    public interface ICharacterAttackUseCase
    {
        IReadOnlyReactiveProperty<bool> IsEquipping { get; }
        IReadOnlyReactiveProperty<bool> IsAttacking { get; }
        IReadOnlyReactiveProperty<bool> IsSpinAttacking { get; }
        IReadOnlyReactiveProperty<bool> IsEquipped { get; }
        IReadOnlyReactiveProperty<bool> IsUnequipping { get; }

        void Tick();
        void EquipStarted();
        void OnEquipped();
        void OnUnequipped();
        void OnAttackRequested();
        void OnUnequipRequested();
        void OnAttackStarted();
        void OnAttackFinished();
        void OnSpinAttackStarted();
        void OnSpinAttackFinished();

        bool CanEquip();
        bool CanUnequip();

        void ForceGiveUpDeadEnemy();
    }
}




