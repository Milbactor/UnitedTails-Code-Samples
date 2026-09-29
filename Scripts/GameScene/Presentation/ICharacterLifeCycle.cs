namespace WhiteKNight
{
    public interface ICharacterLifeCycle
    {
        bool IsDead { get; }
        void OnDead();
    }

}