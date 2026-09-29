using UniRx;


namespace WhiteKNight
{
    public class AICharacterDamagePresenter :  IThirdPersonDamagePresenter, ICharacterLifeCycle
    {
        private IThirdPeresonCharacterDamageUseCase _characterDamageUseCase;
        private HPUseCase _hPUseCase;
        private AICharacterDamageView _characterDamageView;
        private readonly CharacterDeathNotifier _characterDeathNotifier;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        private bool _isDead = false;
        public bool IsDead => _isDead;

        public AICharacterDamagePresenter(
            IThirdPeresonCharacterDamageUseCase characterDamageUseCase, 
            HPUseCase hPUseCase, 
            AICharacterDamageView characterDamageView,
            CharacterDeathNotifier characterDeathNotifier
            )
        {
            _characterDamageUseCase = characterDamageUseCase;
            _characterDamageView = characterDamageView;
            _hPUseCase = hPUseCase;

            _characterDeathNotifier = characterDeathNotifier;
            characterDeathNotifier.Register(this);

            _characterDamageView.OnDamageTaken
                .Subscribe(damage =>
                {
                    _characterDamageUseCase.TakeDamage(damage);
                    _hPUseCase.CurrentHP.Value = _characterDamageUseCase.HP.Value;

                }).AddTo(_disposables);

            _characterDamageUseCase.IsDead
                .SkipLatestValueOnSubscribe()
                .DistinctUntilChanged()
                .Where(x => x)
                .Subscribe(_ =>
                {
                    _characterDeathNotifier.NotifyDead();
                    _characterDamageView.Die();
                })
                .AddTo(_disposables);
        }

        public void OnDead()
        {
            if(_isDead) return;
            _isDead = true;

            _disposables.Dispose();
        }
    }
}