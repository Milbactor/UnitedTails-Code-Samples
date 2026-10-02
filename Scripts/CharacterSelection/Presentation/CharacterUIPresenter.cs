using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public class CharacterUIPresenter :  ICharacterUIPresenter
    {

        private readonly CharacterUIView view;
        private readonly ICharacterSelectionUseCase useCase;

        private readonly CompositeDisposable disposables = new();


        public CharacterUIPresenter(CharacterUIView view, ICharacterSelectionUseCase useCase)
        {
            this.view = view;
            this.useCase = useCase;

            view.OnSelectClicked
                .Subscribe(_ => {
                    useCase.OnCharacterSelected(view.CharacterId);
                    view.Hide();
                })
                .AddTo(disposables);

            view.OnCancelClicked
                .Subscribe(_ => {
                    useCase.OnCharacterNotSelected(view.CharacterId);
                    view.Hide();
                })
                .AddTo(disposables);
        }
    }
}