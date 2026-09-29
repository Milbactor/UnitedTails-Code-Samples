using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace WhiteKNight
{
    public class BallEnemyInstaller : MonoInstaller
    {
        [SerializeField] private BallEnemyView _view;
        [SerializeField] private List<BallEnemyHitBoxView> _hitBoxViews = new List<BallEnemyHitBoxView>();
        [SerializeField] private CharacterDepartureView _departureView;
        [SerializeField] private BallEnemySensorView _sensorView;
        [SerializeField] private BallEnemyAttackAreaView _attackAreaView;
        [SerializeField] private BallResetPositionView _resetPositionView;
        [SerializeField] private OffScreenIndicatorView _offScreenIndicatorView;
        [SerializeField] private float _maxHP = 100f;

        private void Awake()
        {
            Initialize();
        }

        protected virtual void Initialize()
        {
            var model = new BallEnemyModel(_maxHP);
            var useCase = new BallEnemyUseCase(model);
            var presenter = new BallEnemyPresenter(
                useCase,
                _view,
                _hitBoxViews,
                _sensorView,
                _attackAreaView,
                _resetPositionView,
                _offScreenIndicatorView
                );
        }
    }
}