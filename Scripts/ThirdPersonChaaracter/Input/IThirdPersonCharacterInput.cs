using UnityEngine;

namespace WhiteKNight
{
    public interface IThirdPersonCharacterInput
    {
        Vector2 MoveInput { get; }
        float MouseX { get; }
        bool JumpPressed { get; }
        bool AttackPressed { get; }
        bool UnequipPressed { get; }
        void SetTarget(Transform targetTransform);
        Transform Target { get; }
        bool TryClosestTargetPoint(out Vector3 closestPoint);
        void SetInputEnabled(bool enabled);
    }
}

