using UniRx;

namespace WhiteKNight
{
    public class ThirdPersonDamagePresenter : IThirdPersonDamagePresenter
    {
        private readonly IThirdPeresonCharacterDamageUseCase _characterDamageUseCase;
        private readonly HPUseCase _hPUseCase;
        private readonly ThirdPersonCharacterDamageView _characterDamageView;

        private readonly CompositeDisposable _disposables =
            new CompositeDisposable();

        public ThirdPersonDamagePresenter(
            IThirdPeresonCharacterDamageUseCase characterDamageUseCase,
            HPUseCase hPUseCase,
            ThirdPersonCharacterDamageView characterDamageView
        )
        {
            _characterDamageUseCase = characterDamageUseCase;
            _characterDamageView = characterDamageView;
            _hPUseCase = hPUseCase;

            _characterDamageView.OnDamageTaken
                .Subscribe(damage =>
                {
                    _characterDamageUseCase.TakeDamage(damage);
                    _hPUseCase.CurrentHP.Value =
                        _characterDamageUseCase.HP.Value;
                })
                .AddTo(_disposables);

            _characterDamageUseCase.IsDead
                .SkipLatestValueOnSubscribe()
                .DistinctUntilChanged()
                .Where(isDead => isDead)
                .Subscribe(_ =>
                {
                    OnPlayerDead();
                })
                .AddTo(_disposables);
        }
        private void OnPlayerDead()
        {
            _characterDamageView.Die();
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}