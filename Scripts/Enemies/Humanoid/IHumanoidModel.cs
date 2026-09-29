using UniRx;


namespace WhiteKNight
{
    public interface IHumanoidModel : IEnemyModel
    {
        ReactiveProperty<float> AttackCooldownTimer { get; }
        ReactiveProperty<float> DamageCooldownTimer { get; }
    }

}


