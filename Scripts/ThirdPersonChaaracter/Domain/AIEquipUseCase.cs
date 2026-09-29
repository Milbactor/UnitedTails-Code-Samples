using UniRx;

namespace WhiteKNight
{
    public class AIEquipUseCase : IAIEquipUseCase, ICharacterLifeCycle
    {
        readonly private ICharacterStateModel _stateModel;
        readonly private IAIStateModel _aiStateModel;

        public IReadOnlyReactiveProperty<bool> IsEquipping => _stateModel.IsEquipping;
        public IReadOnlyReactiveProperty<bool> IsEquipped => _stateModel.IsEquipped;
        public IReadOnlyReactiveProperty<bool> IsUnequipping => _stateModel.IsUnequipping;

        public IReadOnlyReactiveProperty<bool> ShouldEquip => _aiStateModel.RequestEquip;
        public IReadOnlyReactiveProperty<bool> ShouldUnequip => _aiStateModel.RequestUnequip;

        private bool _isDead = false;
        public bool IsDead => _isDead;

        private CombatSetting _combatSetting = new CombatSetting ();

        private bool _isPendingUnequip;

        public AIEquipUseCase(
            ICharacterStateModel stateModel, 
            IAIStateModel aiStateModel,
            CombatSetting combatSetting,
            CharacterDeathNotifier characterDeathNotifier)
        {
            _stateModel = stateModel;
            _aiStateModel = aiStateModel;
            _combatSetting = combatSetting;
            characterDeathNotifier.Register(this);
        }
       
        public bool DecideShouldEquip(bool hasTarget, float distanceToEnemy)
        {
            float equipDistance = _combatSetting.AttackRange;
            var result = hasTarget &&
                distanceToEnemy <= equipDistance;
            return result;
        }

        private bool CanEquip()
        {
            return !_stateModel.IsEquipped.Value && !_stateModel.IsUnequipping.Value;
        }

        public void OnEquipRequested()
        {
            if (!CanEquip())
                return;
            _aiStateModel.RequestEquip.Value = true;
        }

        public void EquipStarted()
        {
            _aiStateModel.SetMoveState(AIMoveState.ChasingEnemy);
            _stateModel.IsEquipping.Value = true;
        }
       
        public void OnEquipped()
        {
            _stateModel.IsEquipping.Value = false;
            _stateModel.IsEquipped.Value = true;
            _aiStateModel.RequestEquip.Value = false;

            if (_isPendingUnequip)
            {
                _isPendingUnequip = false;
                OnUnequipRequested();
            }
        }

        public bool DecideShouldUnequip(bool hasTarget, float distanceToEnemy)
        {
            float unequipDistance = _combatSetting.UnequipDistance;

            var result = !hasTarget ||
                distanceToEnemy > unequipDistance;
            return result;
        }

        public void OnUnequipRequested()
        {
            if (_stateModel.IsEquipping.Value)
            {
                _isPendingUnequip = true;
                return;
            }

            if (!_stateModel.IsEquipped.Value)
                return;

            if (_stateModel.IsAttacking.Value)
            {
                _isPendingUnequip = true;
                return;
            }

            if (_stateModel.IsSpinAttacking.Value)
            {
                _isPendingUnequip = true;
                return;
            }

            OnUnequipStarted();
        }

        public void OnUnequipStarted()
        {
            _stateModel.IsUnequipping.Value = true;
        }

        public void OnUnequipped()
        {
            _aiStateModel.SetMoveState(AIMoveState.ChasingPartner);

            _stateModel.IsUnequipping.Value = false;
            _stateModel.IsEquipped.Value = false;
            _aiStateModel.RequestUnequip.Value = false;
        }

        public void OnDead()
        {
            if (_isDead) return;

            _isDead = true;
            _stateModel.IsEquipping.Value = false;
            _stateModel.IsUnequipping.Value = false;
            _stateModel.IsEquipped.Value = false;
            _aiStateModel.RequestEquip.Value = false;
            _aiStateModel.RequestUnequip.Value = false;
        }
    }
}