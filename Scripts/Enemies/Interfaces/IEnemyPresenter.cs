using System.Collections.Generic;

namespace WhiteKNight
{
    public interface IEnemyPresenter
    {
        IReadOnlyList<IEnemyHitBoxView> HitBoxViews { get; }
    }
}