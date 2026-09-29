using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public class TreeEnemyUseCase
    {
        private readonly TreeEnemyModel _model;

        private readonly HashSet<Transform> _attackTargets = new();

        private readonly ReactiveProperty<bool> _hasAttackTargets = new(false);
        private readonly Subject<Unit> _onDied = new();
        public IReadOnlyReactiveProperty<bool> IsSporeCoolingDown => _model.IsSporeCoolingDown;
        public IReadOnlyReactiveProperty<float> CurrentHp => _model.CurrentHp;
        public IReadOnlyReactiveProperty<bool> IsDamaged => _model.IsDamaged;
        public IReadOnlyReactiveProperty<bool> HasAttackTargets => _hasAttackTargets;
        public IObservable<Unit> OnDied => _onDied;

        public bool IsDead => _model.IsDead;

        public TreeEnemyUseCase(TreeEnemyModel model)
        {
            _model = model;
        }

        public void TakeDamage(float damage)
        {
            if (_model.IsDead)
                return;

            _model.SetDamaged(true);
            _model.DecreaseHp(damage);

            if (_model.IsDead)
            {
                _attackTargets.Clear();
                _hasAttackTargets.Value = false;
                _onDied.OnNext(Unit.Default);
            }
        }

        public void FinishDamage()
        {
            if (_model.IsDead)
                return;

            _model.SetDamaged(false);
        }

        public void AddAttackTarget(Transform target)
        {
            if (target == null || _model.IsDead)
                return;

            RemoveDestroyedTargets();

            if (_attackTargets.Add(target))
            {
                UpdateHasTargets();
            }
        }

        public void RemoveAttackTarget(Transform target)
        {
            if (target != null)
            {
                _attackTargets.Remove(target);
            }

            RemoveDestroyedTargets();
            UpdateHasTargets();
        }

        public void SetSporeCoolingDown(bool isCoolingDown)
        {
            if ((isCoolingDown)) { _model.StartSporeCooldown(); }
            else { _model.FinishSporeCooldown(); }
        }

        private void RemoveDestroyedTargets()
        {
            _attackTargets.RemoveWhere(target => target == null);
        }

        private void UpdateHasTargets()
        {
            _hasAttackTargets.Value = _attackTargets.Count > 0;
        }
    }
}