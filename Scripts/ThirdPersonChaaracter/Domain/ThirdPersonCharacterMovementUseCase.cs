using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public class ThirdPersonCharacterMovementUseCase : ICharacterMovementUseCase
    {
        private readonly CharactorGroundCheckController _groundCheckController;
        private ICharacterStateModel _stateModel; 
        private CombatSetting _combatSetting = new CombatSetting();
        public IReadOnlyReactiveProperty<bool> IsOnGround => _stateModel.IsOnGround;
        public IReadOnlyReactiveProperty<Vector3> GroundNormal => _stateModel.GroundNormal;

        public IReadOnlyReactiveProperty<bool> ShouldApplyRootMotion => _stateModel.ShouldApplyRootMotion;
        public IReadOnlyReactiveProperty<bool> IsJumping => _stateModel.IsJumping;
        public IReadOnlyReactiveProperty<bool> IsSpinJumping => _stateModel.IsSpinJumping;
        public IReadOnlyReactiveProperty<float> GroundDistance => _stateModel.GroundDistance;

        public ThirdPersonCharacterMovementUseCase(
            CharactorGroundCheckController groundCheckController,
            ThirdPersonStateModel characterStateModel,
            CombatSetting combatSetting
            )
        {
            _groundCheckController = groundCheckController;
            _stateModel = characterStateModel;
            _combatSetting = combatSetting;
        }

        public void Tick()
        {
            var isGrounded = _groundCheckController.IsGrounded();
            _stateModel.IsOnGround.Value = isGrounded;
            if (isGrounded) { _stateModel.IsJumping.Value = false; }

            Vector3 normal = _groundCheckController.GetGroundNormal();
            _stateModel.GroundNormal.Value = normal;

            var checkDistance = _groundCheckController.GetGroundCheckDistance();
            _stateModel.GroundCheckDistance.Value = checkDistance;

            _stateModel.GroundDistance.Value = _groundCheckController.GetGroundDistance();
            _stateModel.VerticalVelocity.Value = _groundCheckController.VerticalVelocity();

            var applyRootMotion = _groundCheckController.CheckRayGroundHit();
            _stateModel.ShouldApplyRootMotion.Value = applyRootMotion;
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