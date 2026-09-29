using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public class ThirdPersonStateModel : ICharacterStateModel
    {
        public ReactiveProperty<Vector3> Position  { get; set; } = new (Vector3.zero);
        public ReactiveProperty<float> VerticalVelocity  { get; set; } = new (0);
        public ReactiveProperty<bool> IsOnGround { get; set; } = new(false);
        public ReactiveProperty<Vector3> GroundNormal { get; set; } = new(Vector3.down);
        public ReactiveProperty<float> GroundCheckDistance { get; set; } = new(5f);
        public ReactiveProperty<bool> ShouldApplyRootMotion { get; set; } = new(true);
        public ReactiveProperty<bool> IsJumping { get; set; } = new(false);
        public ReactiveProperty<bool> IsSpinJumping { get; set; } = new(false);

        public ReactiveProperty<bool> IsAttacking { get; set; } = new(false);
        public ReactiveProperty<bool> IsSpinAttacking { get; set; } = new(false);
        public ReactiveProperty<bool> IsEquipping { get; set; } = new(false);
        public ReactiveProperty<bool> IsEquipped { get; set; } = new(false);
        public ReactiveProperty<bool> IsUnequipping { get; set; } = new(false);
        public ReactiveProperty<float> AttackCooldownTimer { get; set; } = new(0f);
        public ReactiveProperty<float> SpinAttackCooldownTimer { get; set; } = new(0f);
        public ReactiveProperty<bool> IsDead { get; set; } = new(false);
        public ReactiveProperty<float> HP { get; set; } = new(0f);

        public ReactiveProperty<float> GroundDistance  { get; set; } = new (0f);
        public ReactiveProperty<float> DamageCoolDownTimer { get; set; } = new ReactiveProperty<float>(0f);
        
        private float _maxHP = 0f;
        public float MaxHP => _maxHP;

        public ThirdPersonStateModel(CharacterSetting characterSetting)
        {
            _maxHP = characterSetting.CombatSetting.MaxHP;
        }

        public void Reset()
        {
            IsOnGround.Value = false;
            IsJumping.Value = false;
            ShouldApplyRootMotion.Value = true;
            GroundNormal.Value = Vector3.down;
        }
    }
}