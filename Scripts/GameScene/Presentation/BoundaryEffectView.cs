using UnityEngine;

namespace WhiteKNight
{
    public sealed class BoundaryEffectView : MonoBehaviour
    {
        [SerializeField]
        private ParticleSystem blackSporeVfx;

        private void Awake()
        {
            blackSporeVfx?.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        public void StartSpore() 
        {
            blackSporeVfx?.Play();
        }

        public void StopSpore()
        {
            blackSporeVfx?.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
}