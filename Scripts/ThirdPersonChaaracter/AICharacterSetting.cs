using UnityEngine;

namespace WhiteKNight
{
    public class AICharacterSetting : MonoBehaviour, ICharacterSetting
    {
        [SerializeField] private  CombatSetting _combatSetting;
        public CombatSetting CombatSetting => _combatSetting;

        [SerializeField] private AICharacterMovementSettings _characterMovementSettings;
        public AICharacterMovementSettings CharacterMovementSettings => _characterMovementSettings;
    }
}