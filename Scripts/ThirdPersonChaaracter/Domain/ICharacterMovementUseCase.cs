using UnityEngine;
using UniRx;

namespace WhiteKNight
{
    public interface ICharacterMovementUseCase
    {
        IReadOnlyReactiveProperty<bool> IsOnGround { get; }
        IReadOnlyReactiveProperty<Vector3> GroundNormal { get; }

        IReadOnlyReactiveProperty<bool> ShouldApplyRootMotion { get; }

        void Tick();
        void OnJumpStarted();
        void OnSpinJumpStarted();
        void OnJumpFinished();
        void OnSpinJumpFinished();
    }

    public interface ICharacterJumpEligibility
    {
        bool CanStartJump();
        bool CanStartSpinJump();
    }
}

