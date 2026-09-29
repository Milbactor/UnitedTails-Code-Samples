using UnityEngine;

namespace WhiteKNight
{
    [CreateAssetMenu(
        fileName = "CombatSetting",
        menuName = "WhiteKnight/CombatSetting"
    )]
    public class CombatSetting : ScriptableObject
    {
        [Header("Attack")]
        public float AttackRange = 4f;
        public float StopDistance = 3f;
        public float AttackStepForce = 4f;
        public float AttackCooldownTime = 1.2f;

        [Header("SpinAttack")]
        public float SpinAttackStartDistance = 2.5f;
        public float AerialAttackHeightThreshold = 3f;

        [Header("SpinJump")]
        public float SpinJumpRange = 6f;
        public float MinGroundDistance = 2f;

        public float SpinJumpCoolDownTime = 1.2f;
        public float SpinJumpAttackCoolDownTime = 1.2f;

        [Header("Leave")]
        public float EnemyLeaveDistance = 16f;
        public float PartnerLeaveDistance = 14f;

        [Header("Equip")]
        public float EquipBuffer = 10f;

        [Header("Unequip")]
        public float UnequipDistance = 14f;

        [Header("Battle")]
        public float MaxBattleTime = 60f;

        [Header("MaxHP")]
        public float MaxHP = 110f;

        [Header("AttackPower")]
        public float AttackPower = 13f;

        public Color characterColor;

        public Sprite nameSprite;
    }
}