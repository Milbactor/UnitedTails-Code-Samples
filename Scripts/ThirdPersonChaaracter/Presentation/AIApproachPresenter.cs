using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public class AIApproachPresenter : IAIApproachPresenter, IEnemyDeathListener, ICharacterLifeCycle
    {
        private readonly IAIApproachUseCase _approachUseCase;
        private readonly IAIBehaviourInput _input;
        private readonly AICharacterAttackView _view;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        private bool _isDead = false;
        public bool IsDead => _isDead;

        public AIApproachPresenter(
           IAIBehaviourInput input,
            AICharacterAttackView view,
            IAIApproachUseCase approachUseCase,
            IAIStateModel aiStateModel,
            CharacterDeathNotifier characterDeathNotifier
            )
        {
            _input = input;
            _view = view;
            _approachUseCase = approachUseCase;

            characterDeathNotifier.Register(this);

            _view.OnUpdate
                .Where(_ =>  !_isDead)
                .Subscribe(_ => Tick())
                .AddTo(_disposables);
        }

        private void Tick()
        {
            var context = new AIApproachContext(
                hasTarget: _input.HasTarget,
                 distanceToTarget: _input.DistanceToTarget,
                 heightDifferenceToTarget: _input.HeightDistanceToTarget
             );

            _approachUseCase.Tick();
            _approachUseCase.UpdateApproachState(context);

            float distanceToEnemy = context.DistanceToTarget;

            if (_input.HasTarget &&
                _approachUseCase.ShouldChaseEnemy(distanceToEnemy))
            {
                _approachUseCase.OnEnemyFound();
            }

            float distanceToPartner =
                Vector3.Distance(
                    _input.Partner.position,
                    _input.SelfTransform.position);


            if (_approachUseCase.ShouldLeaveEnemy(
                    distanceToPartner,
                    distanceToEnemy))
            {
                GiveUpEnemy();
            }
        }

        private void GiveUpEnemy()
        {
            float distanceToPartner =
                Vector3.Distance(
                    _input.Partner.position,
                    _input.SelfTransform.position);
            _input.GiveUpEnemy();
            _approachUseCase.OnGiveUpEnemyRequested();
        }

        public void OnEnemyDead()
        {
            _input.GiveUpEnemy();
            _approachUseCase.ForceGiveUpDeadEnemy();
        }

        public void OnEnemyDied(IEnemyView enemy)
        {
            _input.GiveUpEnemy();
            _approachUseCase.ForceGiveUpDeadEnemy();
        }

        public void OnDead()
        {
            if (_isDead) return;
            _isDead = true;

            _disposables.Dispose();
        }
    }
}