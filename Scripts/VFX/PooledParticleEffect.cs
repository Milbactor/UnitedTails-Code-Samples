using System;
using UnityEngine;

namespace WhiteKNight
{
    //effectのパーティクルにアタッチ
    //HitEffectPoolから呼び出される
    public class PooledParticleEffect : MonoBehaviour
    {
        private ParticleSystem _particleSystem;
        private Action<PooledParticleEffect> _onFinished;

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        public void Initialize(Action<PooledParticleEffect> onFinished)
        {
            _onFinished = onFinished;
        }

        public void Play()
        {
            gameObject.SetActive(true);

            _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            _particleSystem.Play(true);
        }

        private void OnParticleSystemStopped()
        {
            _onFinished?.Invoke(this);
        }
    }
}