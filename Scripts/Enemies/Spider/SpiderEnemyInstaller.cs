using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace WhiteKNight
{
    public class SpiderEnemyInstaller : MonoInstaller
    {
        [SerializeField] private SpiderEnemyView _view;
        [SerializeField] private List<SpiderEnemyHitBoxView> _hitBoxViews = new List<SpiderEnemyHitBoxView>();
        [SerializeField] private CharacterDepartureView _departureView;
        [SerializeField] private SpiderEnemySensorView _sensorView;
        [SerializeField] private SpiderEnemyAttackAreaView _attackAreaView;

        private void Awake()
        {
            Initialize();
        }

        protected virtual void Initialize()
        {
            _view = GetComponent<SpiderEnemyView>();

            var model = new SpiderEnemyModel();
            var useCase = new SpiderEnemyUseCase(model);
            var presenter = new SpiderEnemyPresenter(useCase, _view, _hitBoxViews, _sensorView, _attackAreaView);
        }
    }
}