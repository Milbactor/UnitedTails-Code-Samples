using UniRx;

namespace WhiteKNight
{
    public interface IBallEnemyUseCase : IEnemyUseCase
    {
        ReactiveProperty<bool> IsJumping { get; }
        ReactiveProperty<bool> IsWaitingForNextJump { get; }

        void OnDamageEnded();
        bool CanMoveToDestination();
        void OnJumpStarted();
        void OnJumpFinished();
    
    }
}