using UniRx;

namespace WhiteKNight
{
    public class ThirdPersonCharacterGameOverPresenter
    {
        private readonly IThirdPeresonCharacterDamageUseCase _damageUseCase;
        private readonly ICameraView _cameraView;
        private readonly ResultUIView _resultUIView;
        private readonly IThirdPersonCharacterInput _characterInput;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        public ThirdPersonCharacterGameOverPresenter(
            IThirdPeresonCharacterDamageUseCase damageUseCase,
            ICameraView cameraView,
            ResultUIView resultUIView,
            IThirdPersonCharacterInput thirdPersonCharacterInput
           )
        {
            _damageUseCase = damageUseCase;
            _cameraView = cameraView;
            _resultUIView = resultUIView;
            _characterInput = thirdPersonCharacterInput;

            _damageUseCase.IsDead
                .SkipLatestValueOnSubscribe()
                .DistinctUntilChanged()
                .Where(isDead => isDead)
                .Subscribe(_ =>
                {
                    Observable.Timer(System.TimeSpan.FromSeconds(1.5f))
                        .Subscribe(_ =>
                        {
                            _characterInput.SetInputEnabled(false);
                            _cameraView.LockFollowPosition();
                            _resultUIView.gameObject.SetActive(true);
                            _resultUIView.OnLost();
                        })
                        .AddTo(_disposables);
                })
                .AddTo(_disposables);
        }
    }
}
