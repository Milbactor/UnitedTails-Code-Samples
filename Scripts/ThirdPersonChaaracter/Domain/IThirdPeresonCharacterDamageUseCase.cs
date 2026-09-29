using UniRx;

namespace WhiteKNight
{
    public interface IThirdPeresonCharacterDamageUseCase
    {
        IReadOnlyReactiveProperty<bool> IsDead { get; }
        IReadOnlyReactiveProperty<float> HP { get; }
        void TakeDamage(float damage);

    }
}