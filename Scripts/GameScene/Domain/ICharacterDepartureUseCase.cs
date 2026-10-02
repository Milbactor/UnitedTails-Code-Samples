using UniRx;

namespace WhiteKNight
{
    public interface ICharacterDepartureUseCase
    { 
        ReactiveProperty<bool> IsDead { get; }
    }
}