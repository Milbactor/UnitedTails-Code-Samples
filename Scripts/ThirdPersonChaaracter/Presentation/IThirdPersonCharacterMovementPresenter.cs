using UniRx;

namespace WhiteKNight
{
    public interface IThirdPersonCharacterMovementPresenter 
    { 
        IReadOnlyReactiveProperty<float> GroundDistance { get; } 
    }
}