using UniRx;

namespace WhiteKNight
{
    public class CharacterInputPresenter : ICharacterInputPresenter
    {
        private readonly CompositeDisposable disposables = new();

        public CharacterInputPresenter(ICharacterInputView view, ICharacterSelectionUseCase useCase)
        {
            view.OnHoverEnter
                .Subscribe(id => useCase.OnHoverEnter(id))
                .AddTo(disposables);

            view.OnHoverExit
                .Subscribe(id => useCase.OnHoverExit(id))
                .AddTo(disposables);

            view.OnClicked
                .Subscribe(id => useCase.OnCliked(id))
                .AddTo(disposables);
        }
    }
}