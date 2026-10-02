using UnityEngine;

namespace WhiteKnight
{
    public class CharacterCameraController : MonoBehaviour, ICharacterCameraController
    {
        [SerializeField] private GameObject mainCamera;

        [System.Serializable]
        private class CharacterUIBinding
        {
            public string characterId;
            public GameObject characterCameraPos;
        }

        [SerializeField]
        private CharacterUIBinding[] bindings;


        public void Show(string characterId)
        {
            mainCamera.SetActive(false);
            var binding = System.Array.Find(bindings, b => b.characterId == characterId);
            if (binding == null)
            {
                Debug.LogWarning($"ProfileUI binding not found for ID: {characterId}");
                return;
            }
            binding.characterCameraPos.SetActive(true);
        }

        public void Hide(string characterId)
        {
            mainCamera.SetActive(true);
            var binding = System.Array.Find(bindings, b => b.characterId == characterId);
            if (binding == null)
            {
                Debug.LogWarning($"ProfileUI binding not found for ID: {characterId}");
                return;
            }

            binding.characterCameraPos.SetActive(false);
        }
    }
}
