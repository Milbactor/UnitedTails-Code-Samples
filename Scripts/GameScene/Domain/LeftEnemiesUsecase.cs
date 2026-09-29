using UniRx;

namespace WhiteKNight
{
    public class LeftEnemiesUseCase : ILeftEnemiesUseCase
    {
        private readonly ILeftEnemiesModel _leftEnemiesModel;

        public IReadOnlyReactiveProperty<int> TotalEnemies => _leftEnemiesModel.TotalEnemies;
        public IReadOnlyReactiveProperty<int> LeftEnemies => _leftEnemiesModel.LeftEnemies;

        public bool IsAllEnemyDead => _leftEnemiesModel.IsAllEnemyDead;

        public LeftEnemiesUseCase(ILeftEnemiesModel leftEnemiesModel)
        {
            _leftEnemiesModel = leftEnemiesModel;
        }

        public void InitializeEnemiesCounters(int total)
        {
            total = UnityEngine.Mathf.Max(0, total);

            _leftEnemiesModel.TotalEnemies.Value = total;
            _leftEnemiesModel.LeftEnemies.Value = total;
        }

        public void OnEnemyDied()
        {
            _leftEnemiesModel.LeftEnemies.Value--;
        }
    }
}
