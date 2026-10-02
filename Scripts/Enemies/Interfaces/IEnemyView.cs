using System;
using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public interface IEnemyView
    {
        void TakeDamage(DamageGivenInfo damageGivenInfo);
        IObservable<DamageGivenInfo> OnDamageTaken { get; }
        void OnTakeDamageStarted(float hpRate);
        void ShowGiveDamageEffect(Vector3 position, Vector3 normal);
        void Die();
        bool IsDead { get; }
        Collider CombatCollider { get; }

    }

    public interface IEnemyAttackView
    {
        void StartAttack();
        void FinishAttack();
        IObservable<Unit> OnAttackFinished { get; }
        float AttackRange { get; }
    }

    public interface IEnemyMoveView
    {
    }
    
    public interface IEnemyDamageViw
    {
        void TakeDamage(DamageGivenInfo damageGivenInfo);
        IObservable<DamageGivenInfo> OnDamageTaken { get; }
        void OnTakeDamageStarted(float hpRate);
        void ShowGiveDamageEffect(Vector3 position, Vector3 normal);
    }
}