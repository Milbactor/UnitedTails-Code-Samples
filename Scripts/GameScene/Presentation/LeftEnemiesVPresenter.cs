using UniRx;

namespace WhiteKNight
{
    public class LeftEnemiesVPresenter : ILeftEnemiesVPresenter
    {
        private readonly ILeftEnemiesUseCase _leftEnemiesUsecase;
        private readonly LeftEnemiesView _leftEnemiesView;
        private readonly CompositeDisposable _disposables = new();

        public LeftEnemiesVPresenter(
            ILeftEnemiesUseCase leftEnemiesUsecase,
            LeftEnemiesView leftEnemiesView)
        {
            _leftEnemiesUsecase = leftEnemiesUsecase;
            _leftEnemiesView = leftEnemiesView;

            Bind();
        }

        private void Bind()
        {
            _leftEnemiesUsecase.TotalEnemies
                .Where(total => total > 0)
                .Take(1)
                .Subscribe(total =>
                {
                    _leftEnemiesView.InitializeDigits(total);
                }).AddTo(_disposables);

            _leftEnemiesUsecase.LeftEnemies
                 .SkipLatestValueOnSubscribe()
                .Subscribe(leftEnemyCount =>
                {
                    float rate = (float) _leftEnemiesUsecase.LeftEnemies.Value /(float) _leftEnemiesUsecase.TotalEnemies.Value;
                    _leftEnemiesView.SetEnemyRate(rate);                 
                    int defeated = _leftEnemiesUsecase.TotalEnemies.Value - leftEnemyCount;
                    _leftEnemiesView.SetDefeatedDigits(defeated);
                })
                .AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
