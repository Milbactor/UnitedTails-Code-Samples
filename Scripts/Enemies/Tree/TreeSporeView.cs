using System.Collections.Generic;
using UnityEngine;

namespace WhiteKNight
{
    public class TreeSporeView : MonoBehaviour, ITreeSporeView
    {
        [SerializeField] private List<ParticleSystem> _particles;
        [SerializeField] private List<TreeSporeHitBoxView> _hitViews;

        public IReadOnlyList<TreeSporeHitBoxView> HitViews => _hitViews;

        public void PlayAttack()
        {
            foreach (ParticleSystem particle in _particles)
            {
                if (particle != null && !particle.isPlaying)
                {
                    particle.Play();
                }
            }
            foreach(var hitView in _hitViews)
            {
                hitView.SetColliderActive(true);
            }
        }

        public void StopEmission()
        {
            foreach (ParticleSystem particle in _particles)
            {
                particle.Stop(
                    true,
                    ParticleSystemStopBehavior.StopEmitting);
            }
            foreach (var hitView in _hitViews)
            {
                hitView.SetColliderActive(false);
            }
        }

        public void DisableAll()
        {
            foreach (var hitView in _hitViews)
            {
                hitView.gameObject.SetActive(false);
            }
        }
    }
}