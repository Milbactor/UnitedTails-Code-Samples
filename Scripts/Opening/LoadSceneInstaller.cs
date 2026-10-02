using UnityEngine;
using Zenject;

namespace WhiteKNight
{
    public class LoadSceneInstaller : MonoInstaller
    {
        [SerializeField] private TransitionView transitionView;

        public override void InstallBindings()
        {
            Container.Bind<ITransitionView>().FromInstance(transitionView).AsSingle();

            LoadSceneUseCase useCase = new LoadSceneUseCase();

            var presenter = Container.Instantiate<TransitionPresenter>(
                new object[] { transitionView, useCase }
            );
        }
    }
}

