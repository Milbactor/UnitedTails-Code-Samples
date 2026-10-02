namespace WhiteKNight
{
    public interface ICharacterAnimatorController
    {
        void PlaySelectSequence(string characterId);
        void PlayDeselectSequence(string characterId);
        void PlayHoveredSequence(string characterId);
        void PlayUnhoveredSequence(string characterId);
    }

}