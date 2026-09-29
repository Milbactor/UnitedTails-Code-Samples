using UniRx;

namespace WhiteKNight
{
    public class WeaponSwapPresenter : IWeaponSwapPresenter
    {
        private WeaponSwapView weaponSwapView;
        private CharacterUIView characterUiView;
        private CharacterInputView characterInputView;
        private readonly CompositeDisposable disposables = new();

        public WeaponSwapPresenter(WeaponSwapView weaponSwapView, 
            CharacterInputView inputView,
            CharacterUIView uiView,
            CharacterDeselectButtonView deselectButtonView
            )
        {
            this.weaponSwapView = weaponSwapView;
            characterInputView = inputView;
            characterUiView = uiView;

            characterInputView.OnClicked
                .Subscribe(id => ShowSwordInHand())
                .AddTo(disposables);

            characterUiView.OnCancelClicked
                .Subscribe( id => ShowSwordOnBack())
                .AddTo(disposables);

            deselectButtonView.OnButtonClicked
                .Subscribe(id => weaponSwapView.SetSwordOnBack())
                .AddTo(disposables);
        }

        public void ShowSwordInHand()
        {
            weaponSwapView.SetSwordInHand();
        }

        public void ShowSwordOnBack()
        {
            weaponSwapView.SetSwordOnBack();
        }
    }
}