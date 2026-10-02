using System;

namespace WhiteKNight
{
    public interface IEnemyHitBoxView
    {
        IObservable<DamageGivenInfo> OnDamageGiven { get; }
        IObservable<DamageGivenInfo> OnDamageTaken { get; }
    }
}