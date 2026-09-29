using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public class AIJumpDecisionUseCase : IAIJumpDecisionUseCase
    {
        private readonly IAIStateModel _aiStateModel;
        private readonly ICharacterStateModel _stateModel;
        private readonly IAIBehaviourInput _input;

        private readonly CharacterObstacleDetectionController
            _obstacleDetectionController;

        private readonly CombatSetting _combatSetting;

        private float _spinCooldownTimer;

        public IReadOnlyReactiveProperty<bool> ShouldJump =>
            _aiStateModel.RequestJump;
        public IReadOnlyReactiveProperty<bool> ShouldSpinJump =>
            _aiStateModel.RequestSpinJump;

        public IReadOnlyReactiveProperty<float> GroundDistance => _stateModel.GroundDistance;
        public AIMoveState moveState => _aiStateModel.CurrentMoveState;

        public AIJumpDecisionUseCase(
            IAIStateModel aiStateModel,
            ICharacterStateModel stateModel,
            IAIBehaviourInput input,
            CharacterObstacleDetectionController
                obstacleDetectionController,
            CombatSetting combatSetting)
        {
            _aiStateModel = aiStateModel;
            _stateModel = stateModel;
            _input = input;

            _obstacleDetectionController =
                obstacleDetectionController;
            _combatSetting = combatSetting;
        }

        public void Tick(Vector3 moveTargetPosition)
        {
            CountDownSpinCooldown();

            _aiStateModel.RequestSpinJump.Value =
                DecideSpinJump(moveTargetPosition);

            _aiStateModel.RequestJump.Value =
                DecideJump(moveTargetPosition);
        }

        private bool DecideJump(Vector3 moveTargetPosition)
        {
            bool partnerJumping = _input.IsPartnerJumping || _input.IsPartnerSpinJumping;

            if (!_stateModel.IsOnGround.Value)
            {
                return false;
            }

            if (_stateModel.IsJumping.Value)
            {
                return false;
            }
            if (_stateModel.IsSpinJumping.Value)
            {

                return false;
            }
            if (_aiStateModel.CurrentMoveState == AIMoveState.Attacking)
            {
                //Jump blocked: MoveState is Attacking
                if (partnerJumping)
                
                    return false;
            }

            bool partnerJump =
                _aiStateModel.CurrentMoveState ==
                    AIMoveState.ChasingPartner &&
            (
                _input.IsPartnerJumping ||
                _input.IsPartnerSpinJumping
            );


            bool blockedFront =
                _obstacleDetectionController
                    .IsFrontBlocked();

            bool leftFree =
                _obstacleDetectionController
                    .IsLeftFree();

            bool rightFree =
                _obstacleDetectionController
                    .IsRightFree();

            bool obstacleJump =
                blockedFront &&
                !leftFree &&
                !rightFree;

            Vector3 ownPosition =
                _input.SelfTransform.position;

            float heightDifference =
                moveTargetPosition.y -
                ownPosition.y;

            bool needsHeightCatchUp =
                heightDifference > _combatSetting.SpinJumpRange;

            bool closeToEnemy =
                _input.HasTarget &&
                _input.DistanceToTarget <=
                    _input.StopDistance;

            var result =
                　partnerJump ||
                  obstacleJump ||
                  needsHeightCatchUp ||
                  closeToEnemy;

            return result;
        }

        private bool DecideSpinJump(
            Vector3 moveTargetPosition)
        {
            // common condition
            if (_stateModel.IsOnGround.Value)
                return false;

            if (_stateModel.GroundDistance.Value <=
                _combatSetting.MinGroundDistance)
                return false;

            if (_stateModel.IsSpinJumping.Value)
                return false;

            if (_stateModel.IsSpinAttacking.Value)
                return false;

            switch (_aiStateModel.CurrentMoveState)
            {
                // to follow partner
                case AIMoveState.ChasingPartner:
                    return DecidePartnerFollowSpinJump(
                        moveTargetPosition);

                // to approach enemy
                case AIMoveState.ChasingEnemy:
                case AIMoveState.Attacking:
                    if (_spinCooldownTimer > 0f)
                        return false;

                    return DecideCombatSpinJump(
                        moveTargetPosition);

                default:
                    return false;
            }
        }
        private bool DecideCombatSpinJump(Vector3 moveTargetPosition)
        {
            return
                _input.HasTarget &&
                _input.DistanceToTarget <=
                    _input.SpinJumpRange;
        }

        private bool DecidePartnerFollowSpinJump(Vector3 moveTargetPosition)
        {
            if (_input.IsPartnerSpinJumping)
                return true;

            float heightDifference =
                moveTargetPosition.y -
                _input.SelfTransform.position.y;

            return heightDifference >
                   _combatSetting.SpinJumpRange;
        }

        public void OnSpinStarted()
        {
            AIMoveState currentMoveState =
                    _aiStateModel.CurrentMoveState;

            if (currentMoveState == AIMoveState.ChasingEnemy ||
                currentMoveState == AIMoveState.Attacking)
            {
                _spinCooldownTimer =
                    _combatSetting.SpinJumpCoolDownTime;
            }
        }
        private void CountDownSpinCooldown()
        {
            _spinCooldownTimer =
                Mathf.Max(
                    0f,
                    _spinCooldownTimer -
                    Time.deltaTime);
        }
    }
}