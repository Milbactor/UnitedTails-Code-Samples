using System;
using UniRx;

namespace WhiteKNight
{
    public class CharacterDeparturePresenter 
    {
        private ICharacterDepartureUseCase _useCase;
        private CharacterDepartureView _view;

        private const float waitTimeForDeparture = 4f;
        private const float deactivateHeight = -15f;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public CharacterDeparturePresenter(ICharacterDepartureUseCase useCase, CharacterDepartureView view)
        {
            _useCase = useCase;
            _view = view;

           _useCase.IsDead
               .Where(x => x)
               .Delay(TimeSpan.FromSeconds(waitTimeForDeparture))
               .Subscribe(_ =>
               {
                   _view.StartDeparture();
                   Observable.EveryUpdate()
                     .Where(_ => _view?.transform.position.y <= deactivateHeight)
                     .Take(1)
                     .Subscribe(_ =>
                     {
                         _view?.gameObject.SetActive(false);
                     })
                     .AddTo(_disposables);
               })
               .AddTo(_disposables);
        }
    }
}