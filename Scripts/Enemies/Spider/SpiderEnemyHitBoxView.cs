using System;
using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public class SpiderEnemyHitBoxView : MonoBehaviour, IEnemyHitBoxView
    {
        [SerializeField] private float _power = 10f;

        private readonly Subject<DamageGivenInfo> _onDamageGiven = new Subject<DamageGivenInfo>();
        public IObservable<DamageGivenInfo> OnDamageGiven => _onDamageGiven;

        private readonly Subject<DamageGivenInfo> _onDamageTaken = new Subject<DamageGivenInfo>();
        public IObservable<DamageGivenInfo> OnDamageTaken => _onDamageTaken;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Weapon"))
            {
                var hitPoint = other.ClosestPoint(other.transform.position);
                _onDamageTaken.OnNext(new DamageGivenInfo(this.gameObject.transform.root.gameObject, hitPoint, _power));
                return;
            }
            if (other.CompareTag("Player"))
            {
                var hitPoint = other.ClosestPoint(other.transform.position);
                _onDamageGiven.OnNext(new DamageGivenInfo(other.gameObject, hitPoint, _power));
                return;
            }
        }

        public float GetCurrentDamage(float hpRate)
        {
            if (hpRate <= 0.25f)
                return _power * 0.5f;

            if (hpRate <= 0.5f)
                return _power * 0.75f;

            return _power;
        }
    }
}