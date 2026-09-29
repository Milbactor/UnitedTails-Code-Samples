using UniRx;

namespace WhiteKNight
{
    public interface IHumanoidEnemyUseCase
    {
        bool IsDead { get; }

        ReactiveProperty<bool> IsAttacking { get; }
        ReactiveProperty<bool> ShouldDecideNewDestination { get; }
        ReactiveProperty<bool> IsIdle { get; }
        ReactiveProperty<bool> IsDamaged { get; }
        ReactiveProperty<bool> CanMove { get; }

        IReadOnlyReactiveProperty<float> AttackCooldownTimer { get; }
        IReadOnlyReactiveProperty<float> DamageCooldownTimer { get; }

        float GetHPRate();

        void OnTakeDamage(float damage);

        void StartDamage(float damageCooldown);
        void EndDamage();

        void Tick(
            float moveInterval,
            float deltaTime);

        void SetNewDestination(bool shouldSet);

        bool CanAttack();

        void StartAttack();
        void EndAttack(float attackCooldown);
    }
}