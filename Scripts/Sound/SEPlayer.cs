using UnityEngine;

namespace WhiteKNight
{
    public class SEPlayer : MonoBehaviour
    {
        public void PlaySE(SoundEffectId soundEffectId)
        {
            SoundManager.Instance?.PlaySE(soundEffectId);
        }
    }
}
