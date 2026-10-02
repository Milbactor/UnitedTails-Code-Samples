using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public class ThirdPersonCharacterAttackPresenter : IThirdPersonCharacterAttackPresenter
    {
        private readonly ICharacterAttackUseCase _useCase;
        private readonly ThirdPersonCharacterInput _input;
        private readonly ThirdPersonCharacterAttackView _view;
        private readonly RotatingSpinAttackEffectView _rotatingSpinAttackEffectView;
        private readonly SwordAttackView _swordAttackView;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public ThirdPersonCharacterAttackPresenter(
                ICharacterAttackUseCase useCase,
                ThirdPersonCharacterInput input,
                ThirdPersonCharacterAttackView view,
                SwordAttackView swordAttackView,
                RotatingSpinAttackEffectView rotatingSpinAttackEffectView
            )
        {
            _useCase = useCase;
            _input = input;
            _view = view;
            _rotatingSpinAttackEffectView = rotatingSpinAttackEffectView;
            _swordAttackView = swordAttackView;

            BindUseCases();
            BindViews();
        }

        private void BindUseCases()
        {
            _useCase.IsAttacking
                .DistinctUntilChanged()
                .Where(x => x)
                .Subscribe(_ =>
                {
                    _view.Attack();
                    if (_input.Target == null) { return; }

                    Vector3 aimPoint;
                    _input.TryClosestTargetPoint(out aimPoint);
                    _view.AttackStepIn(aimPoint);
                })
                .AddTo(_disposables);

            _useCase.IsSpinAttacking
               .DistinctUntilChanged()
               .Where(x => x)
               .Subscribe(_ => {
                   _view.StartSpinAttack();
                   _rotatingSpinAttackEffectView.Play();
               })
               .AddTo(_disposables);

            _useCase.IsEquipping
                .DistinctUntilChanged()
                .Where(isEquipping => isEquipping)
                .Subscribe(_ => _view.Equip())
            .AddTo(_disposables);

            _useCase.IsUnequipping
               .DistinctUntilChanged()
               .Where(isUnequipping => isUnequipping)
               .Subscribe(_ => _view.Unequip())
            .AddTo(_disposables);
        }

        private void BindViews()
        {
            _view.OnUpdate
              .Subscribe(_ =>
              {
                  Tick();
                  _useCase.Tick();
              })
              .AddTo(_disposables);

            _swordAttackView.OnSwordAttackHit.Subscribe(bounceDir =>
            {
                _view.Bounce(bounceDir);
            })
             .AddTo(_disposables);

            _view.OnAttackHitFinished
                .Subscribe(_ =>
                {
                    _useCase.OnAttackFinished();
                })
                .AddTo(_disposables);

            _view.OnEquipFinished
                .Subscribe(_ =>
                {
                    _useCase.OnEquipped();
                })
                .AddTo(_disposables);

            _view.OnUnequipFinished
                 .Subscribe(_ =>
                 {
                     _useCase.OnUnequipped();
                 })
                 .AddTo(_disposables);
            _view.OnSpinAttackFinished
            .Subscribe(_ =>
            {
                _useCase.OnSpinAttackFinished();
            }).AddTo(_disposables);
        }

        public void OnEnemyDead()
        {
            _useCase.ForceGiveUpDeadEnemy();
        }

        void Tick()
        {
            if (_input.AttackPressed)
            {
                _useCase.OnAttackRequested();
                return;
            }

            if (_input.UnequipPressed)
            {
                _useCase.OnUnequipRequested();
            }
        }
    }
}