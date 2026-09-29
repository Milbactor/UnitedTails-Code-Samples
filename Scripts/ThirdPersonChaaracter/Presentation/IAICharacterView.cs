using UnityEngine;

namespace WhiteKNight
{
    public interface IAICharacterView : IThirdPersonCharacterView
    {
        void Move(Vector3 direction, Vector3 groundNormal, AIMovementMode movementMode);
        void SetDestination(Vector3 targetPosition);
    }
}