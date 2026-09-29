using UnityEngine.TextCore.Text;
using WhiteKnight;

namespace WhiteKNight
{
    public class CharacterSelectionUseCase : ICharacterSelectionUseCase
    {
        private readonly ICharacterLightController _lightController;
        private readonly ICharacterUIController _uiController;
        private readonly ICharacterCameraController _cameraController;
        private readonly ICharacterAnimatorController _animatorController;
        private readonly ICharacterModelsController _chracterModelsController;
        private readonly IDialogController _dialogController;

        private readonly ICharacterRepository _repo;

        private readonly ICharacterUIModel _uIModel;
        private readonly ICharacterSelectionModel _characterSelectionModel;

        public CharacterSelectionUseCase(ICharacterUIModel uiModel,
                ICharacterSelectionModel selectionModel,
                ICharacterRepository repo,
                CharacterLightController lightController,
                CharacterAnimatorController animatorController,
                CharacterCameraController cameraController,
                CharacterUIController uiController,
                CharacterModelsController characterModelsController,
                DialogController dialogController)
        {
            _repo = repo;

            _characterSelectionModel = selectionModel;
            _uIModel = uiModel;

            _lightController = lightController;
            _animatorController = animatorController;
            _cameraController = cameraController;
            _uiController = uiController;
            _chracterModelsController = characterModelsController;
            _dialogController = dialogController;
        }

        public void OnHoverEnter(string characterId)
        {
            if (_uIModel.IsSelectionLocked) return;

            _animatorController.PlayHoveredSequence(characterId);
            _uIModel.SetFocus(characterId);
        }

        public void OnHoverExit(string characterId)
        {
            if (_uIModel.IsSelectionLocked) return;

            _animatorController.PlayUnhoveredSequence(characterId);
            _uIModel.Unlock();
        }

        public void OnCliked(string characterId)
        {
            if (_uIModel.IsSelectionLocked) return;

            _uIModel.SetSelectionLocked(true);
            _animatorController.PlaySelectSequence(characterId);

            _chracterModelsController.SetFocusedCharacter(characterId);
            _uiController.Show(characterId);
            _cameraController.Show(characterId);
        }

        public void OnCharacterSelected(string characterId)
        {
            if (_characterSelectionModel.SelectedPlayCharacterId == null)
            {
                _characterSelectionModel.SelectPlayCharacter(characterId);
                _uiController?.Hide(characterId);
                _cameraController?.Hide(characterId);
                _lightController?.EnableLight(characterId, true);
            }
            else if(_characterSelectionModel.SelectedNpcCharacterId == null)
            {
                _characterSelectionModel.SelectNPCCharacter(characterId);
                _uiController?.Hide(characterId);
                _cameraController?.Hide(characterId);
                _lightController?.EnableLight(characterId, true);
            }
            else
            {
                //_dialogController.ShowCanvas();
               // _dialogController.SetButtons(_characterSelectionModel.SelectedPlayCharacterId, _characterSelectionModel.SelectedNpcCharacterId);
            }
            _uIModel?.SetSelectionLocked(false);
            _chracterModelsController?.ResetFocusedCharacter();
        }

        public void OnCharacterDeselected(string deselectedCharacterId)
        {
            _characterSelectionModel.ClearSelectedPlayCharacter(deselectedCharacterId);
            _animatorController?.PlayDeselectSequence(deselectedCharacterId);


            _dialogController.HideCanvas();
            _dialogController.ResetButtons();

            //フォーカス中の戦士を選択にする処理
            var newSelectedCharacterId = _uIModel.FocusedCharacterId;
            UnityEngine.Debug.Log(newSelectedCharacterId);
            _characterSelectionModel.SelectPlayCharacter(newSelectedCharacterId);

            _cameraController?.Hide(newSelectedCharacterId);
            _uiController?.Hide(newSelectedCharacterId);
            _lightController?.EnableLight(newSelectedCharacterId, false);

            _uIModel?.SetSelectionLocked(false);
            _chracterModelsController?.ResetFocusedCharacter();

        }

        public void OnCharacterNotSelected(string characterId)
        {
            _chracterModelsController?.ResetFocusedCharacter();
            _animatorController?.PlayDeselectSequence(characterId);
            _uiController?.Hide(characterId);
            _cameraController?.Hide(characterId);

            _uIModel?.Unlock();
            _uIModel?.SetSelectionLocked(false);
        }
    }

}