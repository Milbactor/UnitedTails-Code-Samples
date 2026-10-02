using UnityEngine;
using Zenject;

namespace WhiteKNight
{
    public class TreeEnemyInstaller : MonoInstaller
    {
        [SerializeField] private TreeEnemyView _view;
        [SerializeField] private TreeSporeView _sporeView;
        [SerializeField] private TreeEnemyAttackAreaView _attackAreaView;
        [SerializeField] private TreeEnemySensorView _sensorView;

        [SerializeField] private float _maxHP = 100f;

        private void Awake()
        {
            Initialize();
        }

        protected virtual void Initialize()
        {
            var model = new TreeEnemyModel(_maxHP);
            var useCase = new TreeEnemyUseCase(model);
            var presenter = new TreeEnemyPresenter(           
                _view,
                _sporeView,
                _attackAreaView,
                _sensorView,
                useCase
                );
        }
    }
}
