using UniRx;

namespace WhiteKNight
{
    public interface IAIAttackUseCase
    {
        IReadOnlyReactiveProperty<bool> IsAttacking { get; }
        IReadOnlyReactiveProperty<bool> IsSpinAttacking { get; }
        IReadOnlyReactiveProperty<bool> RequestAttack { get; }
        IReactiveProperty<bool> RequestSpinAttack { get; }
        IReadOnlyReactiveProperty<bool> SpinAttackRequestedInThisJump { get; }
        void Tick();

        void OnAttackRequested();
        void OnSpinAttackRequested();
        void OnAttackStarted();
        void OnAttackFinished();
        void OnSpinAttackStarted();
        void OnSpinAttackFinished();
        void ForceGiveUpDeadEnemy();

        void OnRequestAttack();
        void OnRequestSpinAttack();

    }
}