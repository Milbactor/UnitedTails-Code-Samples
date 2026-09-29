using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public interface IActorStateProvider
    {
        Vector3 Position { get; }
        IReactiveProperty<bool> IsJumping { get; }
        IReactiveProperty<bool> IsSpinJumping { get; }
        IReactiveProperty<bool> IsGround { get; }
    }
}