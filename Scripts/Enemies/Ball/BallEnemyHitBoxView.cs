using System;
using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public class BallEnemyHitBoxView : MonoBehaviour, IEnemyHitBoxView
    {
        private readonly Subject<DamageGivenInfo> _onDamageGiven = new Subject<DamageGivenInfo>();
        public IObservable<DamageGivenInfo> OnDamageGiven => _onDamageGiven;

        private readonly Subject<DamageGivenInfo> _onDamageTaken = new Subject<DamageGivenInfo>();
        public IObservable<DamageGivenInfo> OnDamageTaken => _onDamageTaken;

        [SerializeField] private float _power = 10f;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Weapon"))
            {
                var hitPoint = other.ClosestPoint(other.transform.position);
                _onDamageTaken.OnNext(new DamageGivenInfo(other.gameObject.transform.root.gameObject, hitPoint, _power));
                return;
            }

            if (other.gameObject.CompareTag("Player"))
            {
                var hitPoint = other.ClosestPoint(other.transform.position);
                _onDamageGiven.OnNext(new DamageGivenInfo(other.gameObject.transform.root.gameObject, hitPoint, _power));
            }
        }
    }
}