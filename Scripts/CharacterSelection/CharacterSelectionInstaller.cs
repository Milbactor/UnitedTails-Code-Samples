using System.Collections.Generic;
using UnityEngine;
using WhiteKnight;
using Zenject;

namespace WhiteKNight
{
    public class CharacterSelectionInstaller : MonoInstaller
    {
        [SerializeField] private List<CharacterInputView> inputViews;
        [SerializeField] private List<CharacterUIView> uiViews;
        [SerializeField] private List<WeaponSwapView> weaponSwapViews;
        [SerializeField] private List<CharacterDeselectButtonView> deselectViews;

        [SerializeField] private CharacterLightController lightController;
        [SerializeField] private CharacterAnimatorController animatorController;
        [SerializeField] private CharacterUIController uiController;
        [SerializeField] private CharacterCameraController cameraController;
        [SerializeField] private CharacterModelsController modelsController;
        [SerializeField] private DialogController dialogController;

        public override void InstallBindings()
        {
            // Model &Repository(bind PureC#s)
            Container.Bind<ICharacterUIModel>().To<CharacterUIModel>().AsSingle();
            Container.Bind<ICharacterRepository>().To<CharacterRepository>().AsSingle();

            //usecase
            var useCase = new CharacterSelectionUseCase(
                Container.Resolve<ICharacterUIModel>(),
                Container.Resolve<ICharacterSelectionModel>(),
                Container.Resolve<ICharacterRepository>(),
                lightController,
                animatorController,
                cameraController,
                uiController,
                modelsController,
                dialogController
            );

            for (int i = 0; i < inputViews.Count; i++)
            {
                var inputView = inputViews[i];
                var uiView = uiViews[i];
                var weaponSwapView = weaponSwapViews[i];
                var deselectView = deselectViews[i];
                
                // Bind View & UseCase
                Container.Bind<ICharacterInputView>().FromInstance(inputView).AsTransient();
                Container.Bind<ICharacterUIView>().FromInstance(uiView).AsTransient();
                Container.Bind<IWeaponSwapView>().FromInstance(weaponSwapView).AsTransient();
                Container.Bind<ICharacterDeselectButtonView>().FromInstance(deselectView).AsTransient();

                var inputPresenter = Container.Instantiate<CharacterInputPresenter>(
                    new object[] { inputView, useCase }
                );
                var uiPresenter = Container.Instantiate<CharacterUIPresenter>(
                    new object[] { uiView, useCase }
                );

                var weaponSwapPresenter = Container.Instantiate<WeaponSwapPresenter>(
                      new object[] { weaponSwapView, inputView, uiView, deselectView }
                );

                var deselectPresenter = Container.Instantiate<CharacterDeselectPresenter>(
                    new object[] { deselectView, useCase }
              );
            }
        }
    }
}