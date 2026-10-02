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
        bool CanStartJump();
        void OnJumpStarted();
        void OnSpinJumpStarted();
        bool CanStartSpinJump();
        void OnJumpFinished();
        void OnSpinJumpFinished();
    }
}

