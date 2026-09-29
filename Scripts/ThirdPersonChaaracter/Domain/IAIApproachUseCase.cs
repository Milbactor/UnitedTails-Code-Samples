using UnityEngine;

namespace WhiteKNight
{
    public interface IAIApproachUseCase
    {
        bool ShouldChaseEnemy(float distanceToEnemy);
        bool ShouldLeaveEnemy(
            float distanceToPartner,
            float distanceToEnemy);

        void OnEnemyFound();
        void OnGiveUpEnemyRequested();
        void ForceGiveUpDeadEnemy();

        void UpdateApproachState(
            AIApproachContext context);

        void Tick();
    }
}