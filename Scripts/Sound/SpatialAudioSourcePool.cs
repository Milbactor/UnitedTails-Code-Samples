using UnityEngine;

namespace WhiteKNight
{
    public class SpatialAudioSourcePool : MonoBehaviour
    {
        [SerializeField] private int poolSize = 3;

        private AudioSource[] _sources;

        private void Awake()
        {
            _sources = new AudioSource[poolSize];

            for (int i = 0; i < poolSize; i++)
            {
                var source = gameObject.AddComponent<AudioSource>();

                source.playOnAwake = false;
                source.spatialBlend = 1f;
                source.rolloffMode = AudioRolloffMode.Custom;
                source.minDistance = 2f;
                source.maxDistance = 20f;

                _sources[i] = source;
            }
        }
        public void Play(SoundEffectEntry data)
        {
            if (data == null || data.Clip == null)
                return;

            for (int i = 0; i < _sources.Length; i++)
            {
                var source = _sources[i];

                if (!source.isPlaying)
                {
                    PlaySource(source, data);
                    return;
                }
            }

            PlaySource(_sources[0], data);
        }

        private void PlaySource(AudioSource source, SoundEffectEntry data)
        {
            source.clip = data.Clip;
            source.volume = data.Volume;
            source.pitch = data.Pitch;
            source.loop = false;
            source.outputAudioMixerGroup = data.Mixer;
            source.Play();
        }
    }
}
