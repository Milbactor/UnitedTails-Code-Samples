namespace WhiteKNight
{
    public interface ICharacterSelectionUseCase
    {
        void OnHoverEnter(string characterId);
        void OnHoverExit(string characterId);
        void OnCliked(string characterId);

        void OnCharacterSelected(string characterId);
        void OnCharacterDeselected(string characterId);
        void OnCharacterNotSelected(string characterId);
    }
}