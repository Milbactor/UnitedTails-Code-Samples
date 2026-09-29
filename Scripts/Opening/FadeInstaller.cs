using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using WhiteKNight;
using Zenject;

namespace WhiteKnight
{
    public class FadeInstaller : MonoInstaller
    {
        [SerializeField] List<FadeView> _views = new List<FadeView>();

        public override void InstallBindings()
        {
            var presenters = _views.Select(v => new FadePresenter(v)).ToList();

            Container.Bind<List<FadePresenter>>()
                .FromInstance(presenters)
                .AsCached();

            Container.Bind<FadeUseCase>()
                .AsCached();
        }
    }

}