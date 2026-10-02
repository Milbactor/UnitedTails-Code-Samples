using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public class AICharacterAttackPresenter : IThirdPersonCharacterAttackPresenter, ICharacterLifeCycle, IEnemyDeathListener
    {
        private readonly IAIAttackUseCase _attackUseCase;
        private readonly IAIEquipUseCase _equipUseCase;
        private readonly AIBehaviourInput _input;
        private readonly AICharacterAttackView _view;
        private readonly RotatingSpinAttackEffectView _rotatingSpinAttackView;
        private readonly SwordAttackView _swordAttackView;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        private bool _isDead = false;
        public bool IsDead => _isDead;

        public AICharacterAttackPresenter(
            IAIAttackUseCase attackUseCase,
            IAIEquipUseCase equipUseCase,
            AICharacterAttackView view,
            AIBehaviourInput input,
            SwordAttackView swordAttackView,
            CharacterDeathNotifier characterDeathNotifier,
            RotatingSpinAttackEffectView rotatingSpinAttackView
            )
        {
            _attackUseCase = attackUseCase;
            _equipUseCase = equipUseCase;
            _input = input;
            _view = view;
            _swordAttackView = swordAttackView;
            _rotatingSpinAttackView = rotatingSpinAttackView;
            characterDeathNotifier.Register(this);
 
            _view.OnUpdate.Subscribe(_ => _attackUseCase.Tick())
                .AddTo(_disposables);

            _swordAttackView.OnSwordAttackHit.Subscribe(bounceDir => _view.Bounce(bounceDir))
                .AddTo(_disposables);

            _attackUseCase.RequestAttack.DistinctUntilChanged()
                .Where(x => x)
                .Subscribe(_ => {
                    _attackUseCase.OnAttackRequested();
                })
                .AddTo(_disposables);

            _attackUseCase.RequestSpinAttack.DistinctUntilChanged()
                .Where(x => x)
                .Subscribe(_ => {
                    _attackUseCase.OnSpinAttackRequested();
                })
                .AddTo(_disposables);


            _attackUseCase.IsAttacking
                .DistinctUntilChanged()
                .Where(x => x)
                .Subscribe(_ => {
                    _view.Attack();
                
                    if (_input.Target == null) { return; }

                    Vector3 aimPoint;
                    _input.TryGetClosestTargetPoint(out aimPoint);
                    _view.AttackStepIn(aimPoint);
                })
                .AddTo(_disposables);

            _attackUseCase.IsSpinAttacking
               .DistinctUntilChanged()
               .Where(x => x)
               .Subscribe(_ => {
                   _view.StartSpinAttack();
                   _rotatingSpinAttackView.Play();
               })
               .AddTo(_disposables);

            _view.OnAttackHitFinished
                .Subscribe(_ => {
                    _attackUseCase.OnAttackFinished();
                })
                .AddTo(_disposables);

            _view.OnSpinAttackFinished
                .Subscribe(_ =>
                {
                    _attackUseCase.OnSpinAttackFinished();

                }).AddTo(_disposables);

             _attackUseCase.SpinAttackRequestedInThisJump
                .Where(requested => requested)
                .Subscribe(_ =>
                {
                    _attackUseCase.OnSpinAttackRequested();
                })
                .AddTo(_disposables);

            // after equipping completion, execute attack which player wanted to
            _equipUseCase.IsEquipped
              .DistinctUntilChanged()
              .Where(x => x)
              .Subscribe(_ => { _attackUseCase.OnAttackRequested(); })
              .AddTo(_disposables);
        }

        public void OnDead()
        {
            if( _isDead ) return;
            _isDead = true;
            _view.EndSpinAttack();
            _disposables.Dispose();
        }

        public void OnEnemyDied(IEnemyView enemy)
        {
            _attackUseCase.ForceGiveUpDeadEnemy();
            _view.EndSpinAttack();
            _view.EndSpinAttack();
        }
    }
}