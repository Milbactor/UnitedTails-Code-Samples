using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public class HumanoidEnemyAttackAreaView : MonoBehaviour, IEnemyAttackAreaView
    {
        private readonly Subject<Transform> _onTargetEntered = new();
        private readonly Subject<Transform> _onTargetExited = new();

        private readonly HashSet<Transform> _targets = new();

        public IObservable<Transform> OnTargetEntered =>
            _onTargetEntered;

        public IObservable<Transform> OnTargetExited =>
            _onTargetExited;

        public bool HasTargetInAttackArea =>
            _targets.Count > 0;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            if (_targets.Add(other.transform))
            {
                _onTargetEntered.OnNext(other.transform);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            if (_targets.Remove(other.transform))
            {
                _onTargetExited.OnNext(other.transform);
            }
        }

        private void OnDisable()
        {
            _targets.Clear();
        }

        private void OnDestroy()
        {
            _onTargetEntered.Dispose();
            _onTargetExited.Dispose();
        }
    }
}