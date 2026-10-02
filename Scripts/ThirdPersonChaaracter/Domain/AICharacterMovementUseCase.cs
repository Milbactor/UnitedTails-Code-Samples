using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public class AICharacterMovementUseCase : ICharacterMovementUseCase, ICharacterLifeCycle
    {
        private ICharacterStateModel _stateModel;
        private IAIStateModel _aiStateModel;
        private readonly CharacterGroundStateUpdater _groundStateProvider;
        public IReadOnlyReactiveProperty<bool> IsOnGround => _stateModel.IsOnGround;
        public IReadOnlyReactiveProperty<Vector3> GroundNormal => _stateModel.GroundNormal;
        public IReadOnlyReactiveProperty<bool> ShouldApplyRootMotion => _stateModel.ShouldApplyRootMotion;

        public AICharacterMovementUseCase(
            ICharacterStateModel stateModel,
            IAIStateModel  aIStateModel,
            CharacterGroundStateUpdater groundStateProvider,
            CharacterDeathNotifier characterDeathNotifier
            )
        {
            _groundStateProvider = groundStateProvider;
            _stateModel = stateModel;
            _aiStateModel = aIStateModel;
            characterDeathNotifier.Register(this);
        }

        public void Tick()
        {
            if(_isDead) return;
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