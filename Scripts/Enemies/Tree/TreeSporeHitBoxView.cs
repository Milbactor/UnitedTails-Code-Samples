using System;
using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public class TreeSporeHitBoxView : MonoBehaviour, IEnemyHitBoxView
    {
        private readonly Subject<DamageGivenInfo> _onDamageGiven = new();
        private readonly Subject<DamageGivenInfo> _onDamageTaken = new();

        [SerializeField] private float _damage;
        [SerializeField] private Collider _collider;

        public IObservable<DamageGivenInfo> OnDamageGiven => _onDamageGiven;
        public IObservable<DamageGivenInfo> OnDamageTaken => _onDamageTaken;

        private void Awake()
        {
            SetColliderActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            Vector3 hitPoint = other.ClosestPoint(transform.position);

            _onDamageGiven.OnNext(
                new DamageGivenInfo(
                    other.gameObject,
                    hitPoint,
                    _damage
                )
            );
        }

        public void SetColliderActive(bool isEnabled)
        {
            _collider.enabled = isEnabled;
        }

        public float GetCurrentDamage(float hpRate)
        {
            if (hpRate <= 0.25f)
                return _damage * 0.5f;

            if (hpRate <= 0.5f)
                return _damage * 0.75f;

            return _damage;
        }
    }
}