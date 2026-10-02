namespace WhiteKNight
{
    public class CharacterSelectionModel : ICharacterSelectionModel
    {
        private string _selectedPlayCharacterId;
        private string _selectedNpcCharacterId;

        public string SelectedPlayCharacterId => _selectedPlayCharacterId;

        public string SelectedNpcCharacterId => _selectedNpcCharacterId;

        public void ClearNpcPlayCharacter()
        {
            _selectedNpcCharacterId = null;
        }

        public void ClearSelectedPlayCharacter()
        {
            _selectedPlayCharacterId = null;
        }

        public void ClearSelectedPlayCharacter(string id)
        {
            if (_selectedPlayCharacterId == id)
                _selectedPlayCharacterId = _selectedNpcCharacterId ?? (_selectedNpcCharacterId = id);   //?? は null 合体演算子：左が null なら右を使う

            /*if (_selectedPlayCharacterId == id)
            {
                if(_selectedNpcCharacterId != null)
                {
                    _selectedPlayCharacterId = _selectedNpcCharacterId;
                }
                else
                {
                    _selectedNpcCharacterId = id;
                }
                    
            }*/

        }

        public void SelectNPCCharacter(string id)
        {
            UnityEngine.Debug.Log(id);
            _selectedNpcCharacterId = id;
        }

        public void SelectPlayCharacter(string id)
        {
            UnityEngine.Debug.Log(id);
            _selectedPlayCharacterId = id;
        }
    }

}