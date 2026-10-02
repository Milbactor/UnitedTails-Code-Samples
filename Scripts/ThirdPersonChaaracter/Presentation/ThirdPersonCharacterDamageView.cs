using System;
using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public class ThirdPersonCharacterDamageView : MonoBehaviour, IThirdPersonCharacterDamageView
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Animator _animator;
        [SerializeField] private GameObject _handSword;

        private const float UpPower = 10f;
        private const float BackPower = 7f;

        private readonly Subject<float> _onDamageTaken = new Subject<float>();
        public IObservable<float> OnDamageTaken => _onDamageTaken;

        private void Awake()
        {
            if (_animator == null)
            {
                _animator = GetComponentInParent<Animator>();
                if (_animator == null) { Debug.LogError("Animator component missing"); }

            }
            if (_rigidbody == null)
            {
                _rigidbody = GetComponentInParent<Rigidbody>();
                if (_rigidbody == null) { Debug.LogError("Rigidbody component missing"); }
            }
        }

        public void TakeDamage(float damage)
        {
            _onDamageTaken.OnNext(damage);
        }

        public void Die()
        {
            _animator?.SetTrigger("TriggerDead");
            _rigidbody.isKinematic = false;
            Observable.Timer(System.TimeSpan.FromSeconds(1.5f))
                .Subscribe(_ =>
                {
                    _rigidbody.useGravity = false;
                    _rigidbody.isKinematic = true;
                })
                .AddTo(this);

            if (_handSword != null && _handSword.activeSelf)
            {
                _handSword.transform.SetParent(null, true);

                var rb = _handSword.GetComponent<Rigidbody>();
                if (rb == null) { rb = _handSword.AddComponent<Rigidbody>(); }
                rb.isKinematic = false;

                var col = _handSword.GetComponent<Collider>();
                if (col != null)
                {
                    col.enabled = true;
                    col.isTrigger = false;
                }
            }       
        }

        public void Bounce(Vector3 normal)
        {
            Vector3 bounceVelocity =
                -transform.forward * BackPower +
                Vector3.up * UpPower;

            _rigidbody.velocity = bounceVelocity;
        }
    }
}
