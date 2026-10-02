using UniRx;

namespace WhiteKNight
{
    public class HPPresenter : IHPPresenter
    {
        private HPView _hpView;
        private IHPUseCase _hpUseCase;
        private readonly CompositeDisposable disposables = new();

        private CombatSetting _combatSetting;

        public HPPresenter(IHPUseCase useCase, HPView hpView, CombatSetting combatSetting)
        {
            _hpUseCase = useCase;
            _hpView = hpView;
            _combatSetting = combatSetting;
            _hpView.Initialize(_combatSetting.characterColor);

            _hpUseCase.CurrentHP
                .DistinctUntilChanged()
                .Subscribe(hp =>
                {
                    var rate = hp / _combatSetting.MaxHP;
                  //  UnityEngine.Debug.Log($"[{_combatSetting.name}] HP:{hp} Max:{_combatSetting.MaxHP} Rate:{rate}");
                    _hpView.SetHPRate(rate);
                })     
                .AddTo(disposables);
        }

        private void OnDestroy()
        {
            disposables.Dispose();
        }
    }
}

