using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public interface IAIJumpDecisionUseCase
    {
        void Tick(Vector3 moveTargetPosition);

        void OnSpinStarted();

        IReadOnlyReactiveProperty<bool> ShouldJump { get; }
        IReadOnlyReactiveProperty<bool> ShouldSpinJump { get; }
        IReadOnlyReactiveProperty<float> GroundDistance { get; }
        AIMoveState moveState { get; }
    }
}