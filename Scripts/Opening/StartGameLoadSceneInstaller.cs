using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace WhiteKNight
{
    public class StartGameLoadSceneInstaller : MonoInstaller
    {
        [SerializeField] private List<TransitionView> transitionViews;

        public override void InstallBindings()
        {
            Container.Bind<ILoadSceneUseCase>().To<LoadSceneUseCase>().AsSingle();

            StartGameUseCase useCase = new StartGameUseCase(Container.Resolve<ICharacterSelectionModel>(), Container.Resolve<ILoadSceneUseCase>());

            foreach(var view in transitionViews)
            {
                Container.Bind<ITransitionView>().FromInstance(view).AsTransient();
                var presenter = Container.Instantiate<StartGameTransitionPresenter>(
                    new object[] { view, useCase }
                );
            }
        }
    }
}

