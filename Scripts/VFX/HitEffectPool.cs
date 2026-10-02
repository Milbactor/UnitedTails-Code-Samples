using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace WhiteKNight
{
    //colliderがついてる攻撃源のGameObjectにアタッチ
    //particleは剣（か攻撃減）が敵にHitしたら出るVFX
    //量がいるのでpoolしておく
    public class HitEffectPool : MonoBehaviour
    {
        [SerializeField] private string _effectAddress = "<Character Name>HitEffect";

        private readonly Queue<PooledParticleEffect> _effects = new();

        private const int TotalEffects = 20;

        private async void Awake()
        {
            for (int i = 0; i < TotalEffects; i++)
            {
                var handle = Addressables.InstantiateAsync(_effectAddress, transform);
                var effectObject = await handle.Task;
         
                var effect = effectObject.GetComponent<PooledParticleEffect>();
                effect.Initialize(ReturnToPool);

                effectObject.SetActive(false);
                _effects.Enqueue(effect);
            }
        }

        public void ShowEffect(Vector3 position)
        {
            if (_effects.Count == 0)
                return;

            var effect = _effects.Dequeue();

            effect.transform.position = position;
            effect.Play();
        }

        public void ShowEffect(Vector3 position, Vector3 normal)
        {
            if (_effects.Count == 0)
                return;

            var effect = _effects.Dequeue();

            effect.transform.position = position;
            Quaternion.FromToRotation(Vector3.up, normal);
            effect.Play();
        }

        private void ReturnToPool(PooledParticleEffect effect)
        {
            effect.gameObject.SetActive(false);
            _effects.Enqueue(effect);
        }
    }
}