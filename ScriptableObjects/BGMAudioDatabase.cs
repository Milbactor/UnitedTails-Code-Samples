using System.Collections.Generic;
using UnityEngine;

namespace WhiteKNight
{
    [CreateAssetMenu(
       fileName = "BGMAudioDatabase",
       menuName = "WhiteKnight/Audio/BGM Database"
       )]
    public class BGMAudioDatabase : ScriptableObject
    {
        [SerializeField] private List<BGMEntry> BGMEntries;
        public  bool TryGetClip (BGMId id, out AudioClip clip)
        {
            var data = BGMEntries.Find(x => x.Id == id);
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

        public BGMEntry Get(BGMId id)
        {
            var data = BGMEntries.Find(x => x.Id == id);
            return BGMEntries.Find(x => x.Id == id);
        }
    }
}
