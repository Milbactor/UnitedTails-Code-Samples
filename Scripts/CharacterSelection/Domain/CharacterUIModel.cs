namespace WhiteKNight
{
    public class CharacterUIModel : ICharacterUIModel
    {
        public string FocusedCharacterId { get; private set; }
        public bool IsSelectionLocked { get; private set; }

        public void SetFocus(string id)
        {
            FocusedCharacterId = id;
       
        }

        public void SetSelectionLocked(bool isLocked)
        {
            IsSelectionLocked = isLocked;
        }

        public void Unlock()
        {
            FocusedCharacterId = null;
            IsSelectionLocked = false;
        }
    }

}