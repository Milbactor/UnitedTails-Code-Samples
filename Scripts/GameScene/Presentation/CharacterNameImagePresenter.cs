namespace WhiteKNight
{
    public class CharacterNameImagePresenter
    {
        CharacterNameView _characterNameImageView;

        private CombatSetting _combatSetting;

        public CharacterNameImagePresenter(CharacterNameView characterNameImageView, CombatSetting combatSetting)
        {
            _characterNameImageView = characterNameImageView;
            _combatSetting = combatSetting;
            SetNameImage();
        }

        public void SetNameImage ()
        {
            _characterNameImageView.SetNameImage(_combatSetting.nameSprite);
        }
    }


}
