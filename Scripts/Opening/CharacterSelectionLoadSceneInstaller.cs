using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace WhiteKNight
{
    public class CharacterSelectionLoadSceneInstaller : MonoInstaller
    {
        [SerializeField] private List<TransitionView> transitionViews;

        public override void InstallBindings()
        {
            LoadSceneUseCase useCase = new LoadSceneUseCase();

            foreach (var transitionView in transitionViews)
            {
                Container.Bind<ITransitionView>().FromInstance(transitionView).AsSingle();
                var presenter = Container.Instantiate<TransitionPresenter>(
                   new object[] { transitionView, useCase }
               );
            }
        }
    }
}

