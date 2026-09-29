using System.Collections.Generic;
using UnityEngine;

namespace WhiteKNight
{
    public class FootStepPlayer : MonoBehaviour
    {
        [SerializeField] private List<SoundEffectId> _soundEffectIds;

        [SerializeField] private Vector2 _pitchRange = new(0.95f, 1.05f);
        [SerializeField] private Vector2 _volumeRange = new(0.8f, 1.0f);
        [SerializeField] private SpatialAudioSourcePool _spatialAudioSourcePool;

        private void Start()
        {
            if (_soundEffectIds.Count == 0) this.enabled = false;
        }

        public void PlayFootStep()
        {
            var soundEffectIdIndex = Random.Range(0, _soundEffectIds.Count);
            var pitch = Random.Range(_pitchRange.x, _pitchRange.y);
            var volume = Random.Range(_volumeRange.x, _volumeRange.y);
            var soundEffectId = _soundEffectIds[soundEffectIdIndex];
            var data = SoundManager.Instance?.GetPlayFootStep(soundEffectId, pitch, volume);
            _spatialAudioSourcePool.Play(data);
        }
        
        //TODO there is no more event in animation called FoodStep attached.. for now not to get error message I call this. but figure out if there is any FootStep Event in animation
        public void FoodStep()
        {
            PlayFootStep();
        }
    }
}
