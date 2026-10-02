using System;
using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public class SwordAttackView : CombatSettingViewBase
    {
        [SerializeField] private HitEffectPool _hitEffectPool;
        [SerializeField, Range(1.0f, 3.0f)]
        [Tooltip(
            "Controls the upward launch angle.\n" +
            "1.0: ~45 degrees\n" +
            "1.5: Slightly higher arc (recommended)\n" +
            "2.0: Steep upward launch\n" +
            "3.0: Almost straight up")]
        private float _upwardLaunchMultiplier = 1.5f;

        private readonly Subject<Vector3> _onSwordAttackHit = new Subject<Vector3>();
        public IObservable<Vector3> OnSwordAttackHit => _onSwordAttackHit;

        private float _power;

        protected override void Awake()
        {
            base.Awake();
            _power = CombatSetting.AttackPower;

            if (_hitEffectPool != null) return;
            bool found = TryGetComponent<HitEffectPool>(out _hitEffectPool);
            if (!found) { Debug.LogWarning("hit effect tool component missing"); }
        }

        public void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Enemy")) return;

            var enemyView = other.GetComponentInParent<IEnemyView>();
            var enemyRootPosition = other.gameObject.transform.root.position;

            // get only horizontal direction
            Vector3 horizontal =
                transform.position - enemyRootPosition;
            horizontal.y = 0f;
            horizontal.Normalize();

            // add upper direction
            Vector3 bounceDirection =
               (horizontal + Vector3.up * _upwardLaunchMultiplier).normalized;
            Bounce(bounceDirection);

            //make sure effect not appearing on the surface in case huge collider like DetectSphere
            var hitPoint = other.ClosestPoint(transform.position);
            var outwardDirection = (hitPoint - enemyRootPosition).normalized;
            // to make it look like sword effect put it bit outside
            _hitEffectPool?.ShowEffect(hitPoint + outwardDirection * 0.05f);

            enemyView?.TakeDamage(new DamageGivenInfo(this.gameObject.transform.root.gameObject, hitPoint, _power));
        }

        private void Bounce(Vector3 bounceDirection)
        {
            _onSwordAttackHit.OnNext(bounceDirection);
        }
    }
}




