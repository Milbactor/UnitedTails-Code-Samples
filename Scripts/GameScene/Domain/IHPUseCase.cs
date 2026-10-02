using UniRx;

namespace WhiteKNight
{
    public interface IHPUseCase
    {
        IReactiveProperty<float> CurrentHP { get; }
    }
}

