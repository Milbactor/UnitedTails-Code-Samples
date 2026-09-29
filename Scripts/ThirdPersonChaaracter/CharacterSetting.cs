using UnityEngine;

namespace WhiteKNight
{
    public class CharacterSetting : MonoBehaviour
    {
        [SerializeField] private CombatSetting _combatSetting;
        public CombatSetting CombatSetting => _combatSetting;
    }
}