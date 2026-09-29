using System;
using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public class BallEnemyAttackAreaView : MonoBehaviour, IEnemyAttackAreaView
    {
        private readonly Subject<Transform> _onTargetEntered = new Subject<Transform>();
        private readonly Subject<Transform> _onTargetExited = new Subject<Transform>();
        public IObservable<Transform> OnTargetEntered => _onTargetEntered;
        public IObservable<Transform> OnTargetExited => _onTargetExited;

        private void OnTriggerEnter(Collider other)
        {
            var damageView =
                other.gameObject.GetComponentInParent<IThirdPersonCharacterDamageView>();

            if (damageView == null) return;

            Transform targetTransform =
                ((MonoBehaviour)damageView).transform;

            _onTargetEntered.OnNext(targetTransform);
        }

        private void OnTriggerExit (Collider other)
        {
            var damageView =
                other.gameObject.GetComponentInParent<IThirdPersonCharacterDamageView>();

            if (damageView == null) return;

            Transform targetTransform =
                ((MonoBehaviour)damageView).transform;

            _onTargetExited.OnNext(targetTransform);
        }
    }
}