namespace WhiteKNight
{
    public interface ICharacterUIModel
    {
        string FocusedCharacterId { get; }
        bool IsSelectionLocked { get; }

        void SetSelectionLocked(bool isLocked);

        void SetFocus(string id);
        void Unlock();
    }
}