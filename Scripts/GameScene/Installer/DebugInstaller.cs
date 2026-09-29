using UnityEngine;
using Zenject;

namespace WhiteKNight
{
    [DefaultExecutionOrder(1)]
    public class DebugInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
              //      Container.Bind<CharacterSelectionModel>()
              //          .FromInstance(new CharacterSelectionModel())
              //          .AsSingle();
        }
    }

}

