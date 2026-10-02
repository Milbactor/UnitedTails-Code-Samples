using UnityEngine;


namespace WhiteKNight
{
    public interface IAIBehaviourInput
    {
        Transform Partner { get; }
        Transform Target { get; }
        Transform SelfTransform { get; }
        bool HasTarget { get; }
        float DistanceToTarget { get; }
        bool TryGetClosestTargetPoint(out Vector3 point);
        float HeightDistanceToTarget { get; }
        bool IsPartnerSpinJumping { get; }
        bool IsPartnerJumping { get; }
        float StopDistance { get; }
        float AttackRange { get; }
        float SpinJumpRange { get; }
        float SpinAttackRange { get; }

        Vector3 GetMoveTargetPosition(AIMoveState currentMoveState);

        bool PreferAerialAttack();

        void SetPartner(Transform target);
        void GiveUpEnemy();
    }
}