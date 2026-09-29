using UniRx;

namespace WhiteKNight
{
    public interface IAIEquipUseCase
    {
        IReadOnlyReactiveProperty<bool> IsEquipping { get; }
        IReadOnlyReactiveProperty<bool> IsEquipped { get; }
        IReadOnlyReactiveProperty<bool> IsUnequipping { get; }

        IReadOnlyReactiveProperty<bool> ShouldEquip { get; }
        IReadOnlyReactiveProperty<bool> ShouldUnequip { get; }

        bool DecideShouldEquip(bool hasTarget, float distanceToEnemy);
        bool DecideShouldUnequip(bool hasTarget, float distanceToEnemy);
        void OnEquipRequested();
        void EquipStarted();
        void OnEquipped();
        void OnUnequipStarted();
        void OnUnequipped();
        void OnUnequipRequested();

    }
}