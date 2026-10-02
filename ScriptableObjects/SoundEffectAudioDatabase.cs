using System.Collections.Generic;
using UnityEngine;

namespace WhiteKNight
{
    [CreateAssetMenu(
    fileName = "SoundEffectAudioDatabase",
    menuName = "WhiteKnight/Audio/Sound Effect Database"
    )]

    public class SoundEffectAudioDatabase : ScriptableObject
    {
        [SerializeField]
        private List<SoundEffectEntry> SoundEffectEntries;

        public bool TryGetClip(SoundEffectId id, out AudioClip clip)
        {
            var data = SoundEffectEntries.Find(x => x.Id == id);
            if (data == null)
            {
                clip = null;
                return false;
            }
            else
            {
                clip = data.Clip;
                return true;
            }
        }

        public SoundEffectEntry Get(SoundEffectId id)
        {
            var data = SoundEffectEntries.Find(x => x.Id == id);
            return SoundEffectEntries.Find(x => x.Id == id);
        }
    }
}
