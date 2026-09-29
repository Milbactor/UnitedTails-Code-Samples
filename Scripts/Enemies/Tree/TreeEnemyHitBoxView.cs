using System;
using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public class TreeEnemyHitBoxView : MonoBehaviour, IEnemyHitBoxView
    {
        [SerializeField] private float _damage;

        private readonly Subject<DamageGivenInfo> _onDamageGiven = new();
        public IObservable<DamageGivenInfo> OnDamageGiven => _onDamageGiven;

        private readonly Subject<DamageGivenInfo> _onDamageTaken = new();
        public IObservable<DamageGivenInfo> OnDamageTaken => _onDamageTaken;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Weapon"))
            {
                var hitPoint = other.ClosestPoint(other.transform.position);
                _onDamageTaken.OnNext(new DamageGivenInfo(other.gameObject, hitPoint, _damage));
                return;
            }

            if (other.gameObject.CompareTag("Player"))
            {
                var hitPoint = other.ClosestPoint(other.transform.position);
                _onDamageGiven.OnNext(new DamageGivenInfo(other.gameObject, hitPoint, _damage));
            }
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