using System;
using UniRx;
using UnityEngine;
using Zenject;

namespace WhiteKNight
{
    public class AICharacterDamageView : MonoBehaviour, IThirdPersonCharacterDamageView, ICharacterLifeCycle
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Animator _animator;
        [SerializeField] private GameObject _handSword;

        private const float bounceRecoverTime = 0.3f;
        private const float upPower = 10f;
        private const float backPower = 7f;

        private readonly Subject<float> _onDamageTaken = new Subject<float>();
        public IObservable<float> OnDamageTaken => _onDamageTaken;

        private bool _isDead = false;
        public bool IsDead => _isDead;


        [Inject]
        public void Construct(CharacterDeathNotifier characterDeathNotifier)
        {
            characterDeathNotifier.Register(this);
        }

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
            _rigidbody.isKinematic = true;

            if (_handSword != null && _handSword.activeSelf)
            {
                _handSword.transform.SetParent(null, true);

                var rb = _handSword.GetComponent<Rigidbody>();
                if (rb == null){
                    rb = _handSword.AddComponent<Rigidbody>();
                }
                rb.isKinematic = false;

                var col = _handSword.GetComponent<Collider>();
                if (col != null)
                {
                    col.enabled = true;
                    col.isTrigger = false;
                }
            }

            gameObject.layer = LayerMask.NameToLayer("Corpse");
            gameObject.tag = "Corpse";
        }

        public void OnDead()
        {
            if (_isDead) return;
            _isDead = true;
        }

        public void Bounce(Vector3 normal)
        {
            if (_isDead == false) { _rigidbody.isKinematic = false; }
            else { return; }

            _rigidbody.isKinematic = false;
            Vector3 bounceVelocity =
                normal.normalized * backPower
                + Vector3.up * upPower;

            _rigidbody.velocity = bounceVelocity;
            Observable.Timer(
                TimeSpan.FromSeconds(bounceRecoverTime))
                .Subscribe(_ =>
                {
                    _rigidbody.velocity = Vector3.zero;
                    _rigidbody.angularVelocity = Vector3.zero;

                    _rigidbody.isKinematic = true;
                })
                .AddTo(this);
        }
    }
}