using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace WhiteKNight
{
    public class HumanoidEnemyInstaller : MonoInstaller
    {
        [SerializeField] private HumanoidEnemyView _view;
        [SerializeField] private List<HumanoidEnemyHitBoxView> _hitBoxViews = new List<HumanoidEnemyHitBoxView>();
        [SerializeField] private CharacterDepartureView _departureView;
        [SerializeField] private HumanoidEnemySensorView _sensorView;
        [SerializeField] private HumanoidEnemyAttackAreaView _attackAreaView;

        private void Awake()
        {
            Initialize();
        }

        protected virtual void Initialize()
        {
            _view = GetComponent<HumanoidEnemyView>();

            var model = new HumanoidEnemyModel();
            var useCase = new HumanoidEnemyUseCase(model);
            var presenter = new HumanoidEnemyPresenter(
                useCase,
                _view, 
                _hitBoxViews,
                _sensorView,
                _attackAreaView);
        }
    }
}

