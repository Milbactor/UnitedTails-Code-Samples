
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WhiteKNight
{
    public class CharacterLightController : MonoBehaviour, ICharacterLightController
    {
        [SerializeField] private List<CharacterLightBinding> bindings;

        public void EnableLight(string id, bool on)
        {
            foreach(var characterLight in bindings)
            {
                characterLight.light.enabled = (characterLight.id == id) && on;
            }     
        }
    }

    [Serializable]
    public class CharacterLightBinding
    {
        public string id;
        public Light light;
    }
}