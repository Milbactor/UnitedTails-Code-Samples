using UnityEngine;

namespace WhiteKNight
{
    public class DialogController : MonoBehaviour, IDialogController
    {
        [SerializeField] private GameObject dialogCanvas;

        [System.Serializable]
        private class CharacterButtonBinding
        {
            public string characterId;
            public GameObject characterButton;
        }

        [SerializeField]
        private CharacterButtonBinding[] bindings;

        public void ShowCanvas() => dialogCanvas.SetActive(true);
        public void HideCanvas() => dialogCanvas.SetActive(false);

        public void SetButtons(string id1, string id2)
        {
            foreach (var bind in bindings)
            {
                bind.characterButton.SetActive(bind.characterId == id1 || bind.characterId == id2);
            }
        }

        public void ResetButtons()
        {
            foreach (var bind in bindings)
            {
                bind.characterButton.SetActive(false); 
            }
        }
    }
}