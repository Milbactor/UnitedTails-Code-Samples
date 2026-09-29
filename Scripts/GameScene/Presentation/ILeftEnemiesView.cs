namespace WhiteKNight
{
    public interface ILeftEnemiesView 
    {
        void InitializeDigits(int total);
        void SetEnemyRate(float rate);
        void SetDefeatedDigits(int remaining);
    }
}