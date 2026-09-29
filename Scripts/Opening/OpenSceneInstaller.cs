using UnityEngine;
using Zenject;

namespace WhiteKNight
{
    public class OpenSceneInstaller : MonoInstaller
    {
        [SerializeField] private OpenSceneTransitionView transitionView;

        public override void InstallBindings()
        {
            Container.Bind<IOpenSceneTransitionView>().FromInstance(transitionView).AsSingle();

            var presenter = Container.Instantiate<OpenSceneTransitionPresenter>(
                new object[] { transitionView }
            );
        }
    }
}

