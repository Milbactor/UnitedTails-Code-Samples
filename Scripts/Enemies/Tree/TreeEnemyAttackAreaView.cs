using System;
using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public class TreeEnemyAttackAreaView : MonoBehaviour, IEnemyAttackAreaView
    {
        private readonly Subject<Transform> _onTargetEntered = new();
        private readonly Subject<Transform> _onTargetExited = new();

        public IObservable<Transform> OnTargetEntered => _onTargetEntered;
        public IObservable<Transform> OnTargetExited => _onTargetExited;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            _onTargetEntered.OnNext(other.transform);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            _onTargetExited.OnNext(other.transform);
        }
    }
}