using System;
using UnityEngine;
using UnityEngine.Audio;

namespace WhiteKNight
{
    public class BGMPlayer : MonoBehaviour
    {
        [SerializeField] private BGMId bgmId;
        [SerializeField] private bool _isPlayOnLoad = true;

        private void Start()
        {
            if (_isPlayOnLoad  == true)
            {
                PlayBGM();
            }
        }

        public void PlayBGM()
        {
           SoundManager.Instance?.PlayBGM(bgmId);
        }
    }

    public enum BGMId
    {
        Title,
        CharacterSelection,
        Game1,
        Game2,
        Battle
    }

    [Serializable]
    public sealed class BGMEntry
    {
        public BGMId Id;
        public AudioClip Clip;
        public float Volume = 1f;
        public float Pitch = 1f;
        public bool Loop;
        public AudioMixerGroup Mixer;
    }
}
