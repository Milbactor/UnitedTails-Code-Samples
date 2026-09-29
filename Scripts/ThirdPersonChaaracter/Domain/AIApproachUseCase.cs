using UnityEngine;

namespace WhiteKNight
{
    public class AIApproachUseCase : IAIApproachUseCase, ICharacterLifeCycle
    {
        private readonly IAIStateModel _aiStateModel;
        private readonly ICharacterStateModel _stateModel;
        private readonly CombatSetting _combatSetting;

        private float _giveUpEnemyTimer;

        public AIApproachUseCase(
            IAIStateModel aiStateModel,
            ICharacterStateModel stateModel,
            CombatSetting combatSetting,
            CharacterDeathNotifier characterDeathNotifier)
        {
            _aiStateModel = aiStateModel;
            _stateModel = stateModel;
            _combatSetting = combatSetting;
            characterDeathNotifier.Register(this);
        }

        public void Tick()
        {
            if (_isDead) return;
            Timer();
        }

        private void Timer()
        {
            _giveUpEnemyTimer =
                Mathf.Max(0f, _giveUpEnemyTimer - Time.deltaTime);
        }

        public void UpdateApproachState(AIApproachContext context)
        {
            if (!context.HasTarget)
            {
                _aiStateModel.SetMoveState(AIMoveState.ChasingPartner);
                return;
            }

            if (context.DistanceToTarget <=
                _combatSetting.EnemyLeaveDistance)
            {
                _aiStateModel.SetMoveState(AIMoveState.ChasingEnemy);
            }
        }

        public bool ShouldChaseEnemy(float distanceToEnemy)
        {
            return
                !_stateModel.IsAttacking.Value &&
                !_stateModel.IsSpinAttacking.Value &&
                distanceToEnemy <= _combatSetting.EnemyLeaveDistance;
        }

        public bool ShouldLeaveEnemy(
           float distanceToPartner,
           float distanceToEnemy)
        {
            if (_aiStateModel.CurrentMoveState == AIMoveState.Attacking ||
                    _stateModel.IsAttacking.Value ||
                    _stateModel.IsSpinAttacking.Value)
                {
                    return false;
                }
                return
                    _giveUpEnemyTimer <= 0f ||
                    distanceToPartner > _combatSetting.PartnerLeaveDistance ||
                    distanceToEnemy > _combatSetting.EnemyLeaveDistance;
            }

        public void OnEnemyFound()
        {
            _giveUpEnemyTimer = _combatSetting.MaxBattleTime;
            _aiStateModel.SetMoveState(AIMoveState.ChasingEnemy);
        }

        public void OnGiveUpEnemyRequested()
        {
            if (_stateModel.IsAttacking.Value)
                return;

            if (_stateModel.IsSpinAttacking.Value)
                return;

            _aiStateModel.SetMoveState(AIMoveState.ChasingPartner);
        }

        public void ForceGiveUpDeadEnemy()
        {
            _giveUpEnemyTimer = 0f;
            _aiStateModel.SetMoveState(AIMoveState.ChasingPartner);
        }

        private bool _isDead = false;
        public bool IsDead => _isDead;
        public void OnDead()
        {
            if (_isDead == true) return;
            _isDead = true;
            _giveUpEnemyTimer = 0f;
            _aiStateModel.SetMoveState(AIMoveState.Dead);
        }
    }
}