using System;
using UniRx;

namespace WhiteKNight
{
    public class AIEquipPresenter : IAIEquipPresenter, ICharacterLifeCycle
    {
        private IAIEquipUseCase _equipUseCase;
        private AICharacterAttackView _attackView;
        private AIBehaviourInput _input;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        private readonly CompositeDisposable AliveDisposables = new CompositeDisposable();

        private bool _isDead = false;
        public bool IsDead => _isDead;

        public AIEquipPresenter(
            IAIEquipUseCase aiEquipUseCase,
            AICharacterAttackView attackView,
            AIBehaviourInput input,
            CharacterDeathNotifier characterDeathNotifier
            )
        {
            _equipUseCase = aiEquipUseCase;
            _attackView = attackView;
            _input = input;

            characterDeathNotifier.Register(this);

            _equipUseCase.ShouldEquip
                .DistinctUntilChanged()
               .Where(x => x)
               .Subscribe(_ =>
               {
                   _equipUseCase.EquipStarted();
               })
           .AddTo(_disposables);

            _equipUseCase.IsEquipping
               .DistinctUntilChanged()
               .Where(x => x)
               .Subscribe(_ =>
               {
                   _attackView.Equip();
               })
           .AddTo(_disposables);

            _equipUseCase.ShouldUnequip
           .DistinctUntilChanged()
               .Where(x => x)
               .Subscribe(_ =>
               {
                   _equipUseCase.OnUnequipRequested();
               })
               .AddTo(_disposables);

            _equipUseCase.IsUnequipping
             .DistinctUntilChanged()
                 .Where(x => x)
                 .Subscribe(_ =>
                 {
                     _attackView.ResetAttackTriggers();
                     _attackView.Unequip();
                 })
                 .AddTo(_disposables);

            _attackView.OnEquipFinished
                .Subscribe(_ =>
                {
                    _equipUseCase.OnEquipped();

                }).AddTo(_disposables);

            _attackView.OnUnequipFinished
                 .Subscribe(_ =>
                 {
                     _equipUseCase.OnUnequipped();
                 })
                 .AddTo(_disposables);

            _equipUseCase.IsUnequipping
                 .DistinctUntilChanged()
                 .Where(x => x)
                 .Subscribe(_ =>
                 {
                     Observable.Timer(TimeSpan.FromSeconds(AICombatDefinition.UnequipDelay))
                         .Subscribe(_ =>
                         {
                             _attackView.ResetAttackTriggers();
                             _equipUseCase.OnUnequipRequested();

                         })  .AddTo(_disposables);

                 })
                 .AddTo(_disposables);

            var equipContext = Observable.EveryUpdate()
                .Select(_ => new
                {
                    HasTarget = input.HasTarget,
                    Distance = input.DistanceToTarget
                })
                .DistinctUntilChanged(x => new
                {
                    x.HasTarget,
                    x.Distance
                })
                .Share();

            equipContext
                .Select(x =>
                    _equipUseCase.DecideShouldEquip(
                        x.HasTarget,
                        x.Distance))
                .DistinctUntilChanged()
                .Where(x => x)
                .Subscribe(_ =>
                {
                    _equipUseCase.OnEquipRequested();
                })
                .AddTo(_disposables);

            equipContext
                .Where(x =>
                    _equipUseCase.DecideShouldUnequip(
                        x.HasTarget,
                        x.Distance))
                .DistinctUntilChanged(x => new
                {
                    x.HasTarget,
                    x.Distance
                })
                .Subscribe(x =>
                {
                    _attackView.ResetAttackTriggers();
                    _equipUseCase.OnUnequipRequested();
                })
                .AddTo(_disposables);
        }

        public void OnDead()
        {
           if(_isDead) return;
           _isDead = true;
            _disposables.Dispose();
        }
    }
}