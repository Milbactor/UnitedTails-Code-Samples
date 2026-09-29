using UnityEngine;

namespace WhiteKNight
{
    public interface ICharacterGroundCheckController
    {
        Vector3 GetGroundNormal();
        bool IsGrounded();
        float GetGroundCheckDistance();
        bool IsAnimatorGroundedState();
        bool CheckRayGroundHit();
    }
}