namespace WhiteKNight
{
    public interface ICharacterSpawnUseCase
    {
        ICharacterModel GetPlayCharacterModel();
        ICharacterModel GetNpcCharacterModel();
    }
}