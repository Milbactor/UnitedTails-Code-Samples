using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public interface ICharacterStateModel
    {
        ReactiveProperty<float> GroundDistance { get; }
        ReactiveProperty<float> VerticalVelocity { get; }
        ReactiveProperty<float> GroundCheckDistance { get; set; }
        ReactiveProperty<Vector3> GroundNormal { get; }
        ReactiveProperty<bool> IsJumping { get; set; }
        ReactiveProperty<bool> IsOnGround { get; set; }
        ReactiveProperty<bool> IsSpinJumping { get; set; }
        ReactiveProperty<bool> ShouldApplyRootMotion { get; set; }
        ReactiveProperty<bool> IsAttacking { get; set; }
        ReactiveProperty<bool> IsSpinAttacking { get; set; }
        ReactiveProperty<bool> IsEquipping { get; set; }
        ReactiveProperty<bool> IsUnequipping { get; set; } 
        ReactiveProperty<bool> IsEquipped { get; set; }
        ReactiveProperty<float> AttackCooldownTimer { get; set; }
        ReactiveProperty<float> SpinAttackCooldownTimer { get; set; }
        ReactiveProperty<float> DamageCoolDownTimer { get; set; }
        ReactiveProperty<float> HP { get; set; }
        float MaxHP { get;}
        ReactiveProperty<bool> IsDead { get; set;}

        void Reset();
    }
}