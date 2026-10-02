using UnityEngine;

namespace WhiteKNight
{
    public class CharacterGroundStateUpdater
    {
        private readonly CharactorGroundCheckController _groundCheckController;

        public CharacterGroundStateUpdater(CharactorGroundCheckController groundCheckController)
        {
            _groundCheckController = groundCheckController;
        }

        public CharacterGroundState GetState()
        {
            return new CharacterGroundState(
                _groundCheckController.IsGrounded(),
                _groundCheckController.GetGroundNormal(),
                _groundCheckController.GetGroundCheckDistance(),
                _groundCheckController.GetGroundDistance(),
                _groundCheckController.VerticalVelocity(),
                _groundCheckController.CheckRayGroundHit()
            );
        }
    }
    public readonly struct CharacterGroundState
    {
        public bool IsGrounded { get; }
        public Vector3 GroundNormal { get; }
        public float GroundCheckDistance { get; }
        public float GroundDistance { get; }
        public float VerticalVelocity { get; }
        public bool ShouldApplyRootMotion { get; }
        public CharacterGroundState(
           bool isGrounded,
           Vector3 groundNormal,
           float groundCheckDistance,
           float groundDistance,
           float verticalVelocity,
           bool shouldApplyRootMotion)
        {
            IsGrounded = isGrounded;
            GroundNormal = groundNormal;
            GroundCheckDistance = groundCheckDistance;
            GroundDistance = groundDistance;
            VerticalVelocity = verticalVelocity;
            ShouldApplyRootMotion = shouldApplyRootMotion;
        }
    }
}