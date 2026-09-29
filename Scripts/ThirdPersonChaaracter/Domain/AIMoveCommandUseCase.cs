using UnityEngine;

namespace WhiteKNight
{
    public class AIMoveCommandUseCase : IAIMoveCommandUseCase
    {
        private readonly IAIStateModel _aiStateModel;
        private readonly CharactorGroundCheckController _groundCheckController;

        public AIMoveCommandUseCase(
            IAIStateModel aiStateModel,
            CharactorGroundCheckController groundCheckController)
        {
            _aiStateModel = aiStateModel;
            _groundCheckController = groundCheckController;
        }

        public void Tick(
            Vector3 ownPosition,
            Vector3 moveTargetPosition)
        {
            DecideMoveDirection(
                ownPosition,
                moveTargetPosition
                );

            DecideMovementMode();
        }

        private void DecideMoveDirection(
            Vector3 ownPosition,
            Vector3 moveTargetPosition)
        {
            Vector3 toTarget =
                moveTargetPosition - ownPosition;

            toTarget.y = 0f;

            float distance =
                toTarget.magnitude;

            if (distance < 0.0001f)
            {
                _aiStateModel.MoveDirection =
                    Vector3.zero;

                return;
            }

            bool shouldMove =
                distance > 3;

            _aiStateModel.MoveDirection =
                shouldMove
                    ? toTarget.normalized
                    : Vector3.zero;
        }

        private void DecideMovementMode()
        {
            bool grounded =
                _groundCheckController.IsGrounded();

            bool hasJumpRequest =
                _aiStateModel.RequestJump.Value ||
                _aiStateModel.RequestSpinJump.Value ||
                _aiStateModel.RequestSpinAttack.Value;


            if (hasJumpRequest)
            {
                _aiStateModel.CurrentMode =
                    AIMovementMode.Physics;
                return;
            }

            _aiStateModel.CurrentMode =
                grounded
                    ? AIMovementMode.NavMesh
                    : AIMovementMode.Physics;
        }

        public AIMoveCommand GetMoveCommand()
        {
            return new AIMoveCommand(
                _aiStateModel.CurrentMode,
                _aiStateModel.MoveDirection);
        }
    }
}