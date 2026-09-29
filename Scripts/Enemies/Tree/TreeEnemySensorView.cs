using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public class TreeEnemySensorView : MonoBehaviour
    {
        [SerializeField] private Transform _owner;
        [SerializeField] private float _searchInterval = 0.1f;
        [SerializeField] private float _switchDistanceThreshold = 0.5f;

        private readonly HashSet<Transform> _targets = new();
        private readonly ReactiveProperty<Transform> _currentTarget = new();

        public IReadOnlyReactiveProperty<Transform> CurrentTarget =>
            _currentTarget;

        private void Awake()
        {
            if (_owner == null)
                _owner = transform;

            Observable.Interval(
                    TimeSpan.FromSeconds(_searchInterval))
                .Subscribe(_ => UpdateNearestTarget())
                .AddTo(this);
        }

        private void OnTriggerEnter(Collider other)
        {
            var damageView =
                other.GetComponentInParent<IThirdPersonCharacterDamageView>();

            if (damageView is not MonoBehaviour targetBehaviour)
                return;

            _targets.Add(targetBehaviour.transform);
            UpdateNearestTarget();
        }

        private void OnTriggerExit(Collider other)
        {
            var damageView =
                other.GetComponentInParent<IThirdPersonCharacterDamageView>();

            if (damageView is not MonoBehaviour targetBehaviour)
                return;

            _targets.Remove(targetBehaviour.transform);
            UpdateNearestTarget();
        }

        private void UpdateNearestTarget()
        {
            _targets.RemoveWhere(target =>
                target == null ||
                !target.gameObject.activeInHierarchy);

            Transform nearestTarget = null;
            float nearestSqrDistance = float.MaxValue;

            foreach (Transform target in _targets)
            {
                float sqrDistance =
                    (target.position - _owner.position).sqrMagnitude;

                if (sqrDistance >= nearestSqrDistance)
                    continue;

                nearestSqrDistance = sqrDistance;
                nearestTarget = target;
            }

            // no target
            if (nearestTarget == null)
            {
                _currentTarget.Value = null;
                return;
            }

            Transform currentTarget = _currentTarget.Value;

            // one candidate exists then its target
            if (currentTarget == null || !_targets.Contains(currentTarget))
            {
                _currentTarget.Value = nearestTarget;
                return;
            }

            // current targer is equal to latest closest candidate
            if (nearestTarget == currentTarget)
                return;

            float currentDistance =
                Vector3.Distance(_owner.position, currentTarget.position);

            float nearestDistance =
                Vector3.Distance(_owner.position, nearestTarget.position);

            // new candidate is closer more 0.5 then change the target
            if (nearestDistance + _switchDistanceThreshold < currentDistance)
            {
                _currentTarget.Value = nearestTarget;
            }
        }
    }
}