using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace WhiteKNight
{
    public class BoundaryInstaller : MonoInstaller
    {
        [SerializeField] private BoundaryDamageView _damageView;
        [SerializeField] private List<BoundaryAreaView> _areaViews = new List<BoundaryAreaView>();
        [SerializeField] private BoundaryEffectView _effectView;

        private void Awake()
        {
            Initialize();
        }

        protected virtual void Initialize()
        {
            var presenter = new BoundaryPresenter(
                _areaViews,
                _damageView,
                _effectView
                );
        }
    }
}