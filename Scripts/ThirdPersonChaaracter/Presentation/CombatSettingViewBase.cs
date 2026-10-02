using UnityEngine;

namespace WhiteKNight
{
    public class CombatSettingViewBase : MonoBehaviour
    {
        [SerializeField] private ICharacterSetting _characterSetting;
        protected CombatSetting CombatSetting { get; private set; }

        protected virtual void Awake()
        {
            if (_characterSetting == null)
            {
                _characterSetting = GetComponentInParent<ICharacterSetting>();
            }

            if (_characterSetting == null)
            {
                Debug.LogError("CharacterSetting not found.", this);
                return;
            }

            CombatSetting = _characterSetting.CombatSetting;
        }
    }
}