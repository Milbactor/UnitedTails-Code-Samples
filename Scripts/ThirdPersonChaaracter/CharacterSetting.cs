using UnityEngine;

namespace WhiteKNight
{
    public class CharacterSetting : MonoBehaviour, ICharacterSetting
    {
        [SerializeField] private CombatSetting _combatSetting;
        public CombatSetting CombatSetting => _combatSetting;

        [SerializeField] private CharacterMovementSettings _characterMovementSettings;
        public CharacterMovementSettings CharacterMovementSettings => _characterMovementSettings;
    }
}