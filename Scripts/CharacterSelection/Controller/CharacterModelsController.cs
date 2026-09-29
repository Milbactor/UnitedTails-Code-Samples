using System;
using System.Collections.Generic;
using UnityEngine;

namespace WhiteKNight
{
    public class CharacterModelsController : MonoBehaviour, ICharacterModelsController
    {
        [SerializeField] List<CharacterModelBinding> bindings = new List<CharacterModelBinding>();

        public void SetFocusedCharacter(string id)
        {
            foreach (var binding in bindings)
            {
                binding.character.SetActive(binding.id == id);
            }
        }
        public void ResetFocusedCharacter()
        {
            foreach (var binding in bindings)
            {
                binding.character.SetActive(true);
            }
        }
    }
    [Serializable]
    public class CharacterModelBinding
    {
        public string id;
        public GameObject character;
    }
}