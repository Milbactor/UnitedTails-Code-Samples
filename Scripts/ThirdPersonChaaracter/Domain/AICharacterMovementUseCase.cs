using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public class AICharacterMovementUseCase : ICharacterMovementUseCase, ICharacterLifeCycle
    {
        private CharactorGroundCheckController _groundCheckController;
        private ICharacterStateModel _stateModel;
        private IAIStateModel _aiStateModel;
        public IReadOnlyReactiveProperty<bool> IsOnGround => _stateModel.IsOnGround;
        public IReadOnlyReactiveProperty<Vector3> GroundNormal => _stateModel.GroundNormal;
        public IReadOnlyReactiveProperty<bool> ShouldApplyRootMotion => _stateModel.ShouldApplyRootMotion;

        public AICharacterMovementUseCase(
            CharactorGroundCheckController groundCheckController,
            ICharacterStateModel stateModel,
            IAIStateModel  aIStateModel,
            CharacterDeathNotifier characterDeathNotifier
            )
        {
            _groundCheckController = groundCheckController;
            _stateModel = stateModel;
            _aiStateModel = aIStateModel;
            characterDeathNotifier.Register(this);
        }

        public void Tick()
        {
            if(_isDead) return;

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

        public bool CanStartJump(){ return !_isDead;} //TODO: change structre in ithirdpersonMoveusecase and add interface for thsese methods

        public bool CanStartSpinJump() { return !_isDead; }

        public void OnJumpStarted()
        {
            _stateModel.IsJumping.Value = true;
        }

        public void OnSpinJumpStarted()
        {
            _stateModel.IsSpinJumping.Value = true;
        }

        public void OnJumpFinished()
        {
            _stateModel.IsJumping.Value = false;
            _aiStateModel.RequestJump.Value = false;
        }

        public void OnSpinJumpFinished()
        {
            _stateModel.IsSpinJumping.Value = false;
            _aiStateModel.RequestSpinJump.Value = false;
        }

        private bool _isDead;
        public bool IsDead => _isDead;
        public void OnDead()
        {
            if (_isDead) return;
            _isDead = true;

            OnJumpFinished();
            OnSpinJumpFinished();
        }
    }
}

