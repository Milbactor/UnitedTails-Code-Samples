using UnityEngine;
using WhiteKNight;

namespace WhiteKnight
{
    public class CharacterUIController : MonoBehaviour, ICharacterUIController
    {
        [SerializeField] private GameObject profileCanvas;

        [System.Serializable]
        private class CharacterUIBinding
        {
            public string characterId;
            public GameObject characterProfile;
        }

        [SerializeField]
        private CharacterUIBinding[] bindings;

        public void Show(string characterId)
        {
            profileCanvas.SetActive(true);
            var binding = System.Array.Find(bindings, b => b.characterId == characterId);
            if (binding == null)
            {
                Debug.LogWarning($"ProfileUI binding not found for ID: {characterId}");
                return;
            }
            binding.characterProfile.SetActive(true);

        }

        public void Hide(string characterId)
        {
            profileCanvas.SetActive(false);
            var binding = System.Array.Find(bindings, b => b.characterId == characterId);
            if (binding == null)
            {
                Debug.LogWarning($"ProfileUI binding not found for ID: {characterId}");
                return;
            }
            binding.characterProfile.SetActive(false);
        }
    }
}
