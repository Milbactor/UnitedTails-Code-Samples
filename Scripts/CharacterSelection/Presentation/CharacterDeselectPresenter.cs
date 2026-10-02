using UniRx;

namespace WhiteKNight
{
    public class CharacterDeselectPresenter : ICharacterDeselectPresenter
    {
        private readonly CompositeDisposable disposables = new();

        public CharacterDeselectPresenter(CharacterDeselectButtonView view, CharacterSelectionUseCase useCase)
        {
            view.OnButtonClicked.Subscribe(_ =>
                useCase.OnCharacterDeselected(view.CharacterId)         
                ).AddTo(disposables);


        }
    }
}