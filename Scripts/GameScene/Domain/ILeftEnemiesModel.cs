using UniRx;

namespace WhiteKNight
{
    public interface ILeftEnemiesModel
    {
        ReactiveProperty<int> TotalEnemies { get; set; }
        ReactiveProperty<int> LeftEnemies { get; set; }

        bool IsAllEnemyDead { get; }
    }


    public class LeftEnemiesModel : ILeftEnemiesModel
    {
        public ReactiveProperty<int> TotalEnemies { get; set; } = new(0);
        public ReactiveProperty<int> LeftEnemies { get; set; } = new(0);

        public bool IsAllEnemyDead => LeftEnemies.Value <= 0;
    }
}
