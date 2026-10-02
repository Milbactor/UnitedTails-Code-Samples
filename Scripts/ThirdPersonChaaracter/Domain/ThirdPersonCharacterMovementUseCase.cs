using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public class ThirdPersonCharacterMovementUseCase : ICharacterMovementUseCase, ICharacterJumpEligibility
    {
        private ICharacterStateModel _stateModel; 
        private readonly CombatSetting _combatSetting = new CombatSetting();
        private readonly CharacterGroundStateUpdater _groundStateProvider;
        public IReadOnlyReactiveProperty<bool> IsOnGround => _stateModel.IsOnGround;
        public IReadOnlyReactiveProperty<Vector3> GroundNormal => _stateModel.GroundNormal;
        public IReadOnlyReactiveProperty<bool> ShouldApplyRootMotion => _stateModel.ShouldApplyRootMotion;
        public IReadOnlyReactiveProperty<bool> IsJumping => _stateModel.IsJumping;
        public IReadOnlyReactiveProperty<bool> IsSpinJumping => _stateModel.IsSpinJumping;
        public IReadOnlyReactiveProperty<float> GroundDistance => _stateModel.GroundDistance;

        public ThirdPersonCharacterMovementUseCase(
           CharacterGroundStateUpdater groundStateProvider,
            ThirdPersonStateModel characterStateModel,
            CombatSetting combatSetting
            )
        {
            _groundStateProvider = groundStateProvider;
            _stateModel = characterStateModel;
            _combatSetting = combatSetting;
        }

        public void Tick()
        {
            var groundState = _groundStateProvider.GetState();

            _stateModel.IsOnGround.Value = groundState.IsGrounded;

            if (groundState.IsGrounded)
            {
                _stateModel.IsJumping.Value = false;
            }

            _stateModel.GroundNormal.Value = groundState.GroundNormal;
            _stateModel.GroundCheckDistance.Value = groundState.GroundCheckDistance;
            _stateModel.GroundDistance.Value = groundState.GroundDistance;
            _stateModel.VerticalVelocity.Value = groundState.VerticalVelocity;
            _stateModel.ShouldApplyRootMotion.Value =
                groundState.ShouldApplyRootMotion;
        }

        public bool CanStartJump()
        {
            return IsOnGround.Value && !IsJumping.Value;
        }
        
        public void OnJumpStarted()
        {
            _stateModel.IsJumping.Value = true;
        }

        public bool CanStartSpinJump()
        {
            return !IsOnGround.Value
                && !IsSpinJumping.Value
                && _stateModel.GroundDistance.Value > _combatSetting.MinGroundDistance;
        }

        public void OnSpinJumpStarted()
        {
            _stateModel.IsSpinJumping.Value = true;
        }

        public void OnJumpFinished()
        {
            _stateModel.IsJumping.Value = false;
        }

        public void OnSpinJumpFinished()
        {
            _stateModel.IsSpinJumping.Value = false;
        }
    }
}