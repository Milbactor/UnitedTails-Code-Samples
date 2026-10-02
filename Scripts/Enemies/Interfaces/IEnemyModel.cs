using UniRx;

namespace WhiteKNight
{
    public interface IEnemyModel
    {
        bool IsDead { get; }
        float MaxHp { get; }
        ReactiveProperty<float> CurrentHp { get; }
        ReactiveProperty<bool> IsAttacking { get; }
        ReactiveProperty<bool> IsIdle { get; }
        ReactiveProperty<bool> IsDamaged { get; }
        ReactiveProperty<bool> IsWalking { get; }
        ReactiveProperty<bool> CanWalk { get; }
        ReactiveProperty<bool> ShouldDecideNewDestination { get; }

        void DecreaseHP(float hp);
        void OnAttackStarted();
        void OnAttackFinished();
        void SetCanMoveState();
    }
}