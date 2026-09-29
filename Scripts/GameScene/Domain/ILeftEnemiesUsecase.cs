using UniRx;

namespace WhiteKNight
{
    public interface ILeftEnemiesUseCase
    {
        IReadOnlyReactiveProperty<int> TotalEnemies { get; }
        IReadOnlyReactiveProperty<int> LeftEnemies { get; }

        bool IsAllEnemyDead { get; }

        void InitializeEnemiesCounters(int total);
        void OnEnemyDied();
    }
}