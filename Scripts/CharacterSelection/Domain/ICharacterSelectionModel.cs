namespace WhiteKNight
{
    public interface ICharacterSelectionModel
    {
        string SelectedPlayCharacterId { get; }
        string SelectedNpcCharacterId { get; }

        void SelectNPCCharacter(string id);
        void SelectPlayCharacter(string id);
        void ClearSelectedPlayCharacter();
        void ClearSelectedPlayCharacter(string id);
        void ClearNpcPlayCharacter();
    }
}