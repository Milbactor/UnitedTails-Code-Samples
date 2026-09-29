#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace WhiteKNight
{
    public static class BGMDatabaseEditor
    {
        [MenuItem("WhiteKnight/Audio/Rebuild BGM Database")]
        public static void Rebuild()
        {
            Debug.Log("Rebuild BGM Database");
        }
    }
}
#endif