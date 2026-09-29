#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace WhiteKNight
{
    public static class SoundEffectDatabaseEditor
    {
        [MenuItem("WhiteKnight/Audio/Rebuild SE Database")]
        public static void Rebuild()
        {
            Debug.Log("Rebuild SE Database");
        }
    }
}
#endif