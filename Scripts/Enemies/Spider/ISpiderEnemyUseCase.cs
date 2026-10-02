using UniRx;

namespace WhiteKNight
{
    public interface ISpiderEnemyUseCase : IEnemyUseCase
    {
        ReactiveProperty<bool> ShouldDecideNewDestination { get; }
        ReactiveProperty<bool> IsAttacking { get; }
        ReactiveProperty<bool> CanWalk { get; }
        ReactiveProperty<bool> IsDamaged { get; }
        void SetNewDestination(bool shouldSet);
        void Tick(float moveInterval, float deltaTime);
        bool CanAttack(); 
        bool IsDead { get; }
        float GetHPRate();
        void OnTakeDamage(float damage);
        void StartAttack();
        void EndAttack();
    }
}


