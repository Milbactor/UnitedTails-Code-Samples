using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public class HumanoidEnemyUseCase : IHumanoidEnemyUseCase
    {
        private readonly IHumanoidModel _humanEnemyModel;

        public bool IsDead => _humanEnemyModel.IsDead;

        public ReactiveProperty<bool> IsAttacking =>
            _humanEnemyModel.IsAttacking;

        public ReactiveProperty<bool> ShouldDecideNewDestination =>
            _humanEnemyModel.ShouldDecideNewDestination;

        public ReactiveProperty<bool> IsIdle =>
            _humanEnemyModel.IsIdle;

        public ReactiveProperty<bool> IsDamaged =>
            _humanEnemyModel.IsDamaged;

        public ReactiveProperty<bool> CanMove =>
            _humanEnemyModel.CanWalk;

        public IReadOnlyReactiveProperty<float> AttackCooldownTimer =>
            _humanEnemyModel.AttackCooldownTimer;

        public IReadOnlyReactiveProperty<float> DamageCooldownTimer =>
            _humanEnemyModel.DamageCooldownTimer;

        private float _moveTimer;

        public HumanoidEnemyUseCase(IHumanoidModel model)
        {
            _humanEnemyModel = model;
        }

        public float GetHPRate()
        {
            if (_humanEnemyModel.MaxHp == 0)
                return 0f;

            return _humanEnemyModel.CurrentHp.Value /
                   _humanEnemyModel.MaxHp;
        }

        public void OnTakeDamage(float damage)
        {
            _humanEnemyModel.DecreaseHP(damage);
        }

        public void StartDamage(float damageCooldown)
        {
            _humanEnemyModel.DamageCooldownTimer.Value =
                damageCooldown;

            _humanEnemyModel.IsDamaged.Value = true;
        }

        public void EndDamage()
        {
            _humanEnemyModel.DamageCooldownTimer.Value = 0f;
            _humanEnemyModel.IsDamaged.Value = false;

        }

        public void Tick(
            float moveInterval,
            float deltaTime)
        {
            UpdateAttackCooldown(deltaTime);
            UpdateDamageCooldown(deltaTime);

            _humanEnemyModel.SetCanMoveState();

            _moveTimer += deltaTime;

            if (_moveTimer >= moveInterval)
            {
                ResetMoveTimer();
            }
        }

        private void UpdateAttackCooldown(float deltaTime)
        {
            _humanEnemyModel.AttackCooldownTimer.Value =
                Mathf.Max(
                    0f,
                    _humanEnemyModel.AttackCooldownTimer.Value -
                    deltaTime
                );
        }

        private void UpdateDamageCooldown(float deltaTime)
        {
            if (!IsDamaged.Value)
                return;

            _humanEnemyModel.DamageCooldownTimer.Value =
                Mathf.Max(
                    0f,
                    _humanEnemyModel.DamageCooldownTimer.Value -
                    deltaTime
                );

            if (_humanEnemyModel.DamageCooldownTimer.Value <= 0f)
            {
                EndDamage();
            }
        }

        private void ResetMoveTimer()
        {
            _moveTimer = 0f;

            _humanEnemyModel
                .ShouldDecideNewDestination.Value = true;
        }

        public void SetNewDestination(bool shouldSet)
        {
            _humanEnemyModel
                .ShouldDecideNewDestination.Value = shouldSet;
        }

        public bool CanAttack()
        {
            if (IsDead)
                return false;

            if (IsDamaged.Value)
                return false;

            if (IsAttacking.Value)
                return false;

            if (_humanEnemyModel.AttackCooldownTimer.Value > 0f)
                return false;

            return true;
        }

        public void StartAttack()
        {
            _humanEnemyModel.IsAttacking.Value = true;
            _humanEnemyModel.CanWalk.Value = false;
        }

        public void EndAttack(float attackCooldown)
        {
            _humanEnemyModel.IsAttacking.Value = false;
            _humanEnemyModel.CanWalk.Value = true;

            _humanEnemyModel.AttackCooldownTimer.Value =
                attackCooldown;
        }
    }
}