using System;
using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public class BoundaryAreaView : MonoBehaviour
    {
        private readonly Subject<ThirdPersonCharacterDamageView>
            _onBoundaryEntered = new();

        private readonly Subject<ThirdPersonCharacterDamageView>
            _onBoundaryExited = new();

        public IObservable<ThirdPersonCharacterDamageView> OnBoundaryEntered =>
            _onBoundaryEntered;

        public IObservable<ThirdPersonCharacterDamageView> OnBoundaryExited =>
            _onBoundaryExited;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;
            ThirdPersonCharacterDamageView damageView =
                other.GetComponentInParent<ThirdPersonCharacterDamageView>();

            if (damageView == null)
                return;
            _onBoundaryEntered.OnNext(damageView);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;
            ThirdPersonCharacterDamageView damageView =
                other.GetComponentInParent<ThirdPersonCharacterDamageView>();

            if (damageView == null)
                return;
            _onBoundaryExited.OnNext(damageView);
        }

        private void OnDestroy()
        {
            _onBoundaryEntered.Dispose();
            _onBoundaryExited.Dispose();
        }
    }
}