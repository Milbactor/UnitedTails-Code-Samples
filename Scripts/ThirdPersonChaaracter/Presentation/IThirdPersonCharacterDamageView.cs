using System;
using UnityEngine;

namespace WhiteKNight
{
    public interface IThirdPersonCharacterDamageView
    {
        void TakeDamage(float damage);
        void Bounce(Vector3 normal);
        void Die();
        IObservable<float> OnDamageTaken { get; }
    }
}
