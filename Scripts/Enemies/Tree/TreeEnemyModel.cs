using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public class TreeEnemyModel
    {
        private readonly ReactiveProperty<float> _currentHp;
        private readonly ReactiveProperty<bool> _isDamaged = new(false);
        private readonly ReactiveProperty<bool> _isSporeCoolingDown = new(false);

        public float MaxHp { get; }

        public IReadOnlyReactiveProperty<float> CurrentHp => _currentHp;
        public IReadOnlyReactiveProperty<bool> IsDamaged => _isDamaged;
        public IReadOnlyReactiveProperty<bool> IsSporeCoolingDown => _isSporeCoolingDown;

        public bool IsDead => _currentHp.Value <= 0f;

        public TreeEnemyModel(float maxHp)
        {
            MaxHp = Mathf.Max(1f, maxHp);
            _currentHp = new ReactiveProperty<float>(MaxHp);
        }

        public void DecreaseHp(float damage)
        {
            if (IsDead || damage <= 0f)
                return;

            _currentHp.Value = Mathf.Max(0f, _currentHp.Value - damage);
        }

        public void SetDamaged(bool isDamaged)
        {
            _isDamaged.Value = isDamaged;
        }

        public void StartSporeCooldown()
        {
            _isSporeCoolingDown.Value = true;
        }

        public void FinishSporeCooldown()
        {
            _isSporeCoolingDown.Value = false;
        }
    }
}