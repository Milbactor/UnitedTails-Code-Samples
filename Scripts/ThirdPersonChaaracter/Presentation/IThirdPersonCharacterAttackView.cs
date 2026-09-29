using UnityEngine;

namespace WhiteKNight
{
    public interface IThirdPersonCharacterAttackView
    {
        void Attack();
        void Equip();
        void OnAttackHitEnd();
        void OnAttackHitStart();
        void Unequip();
        void OnEquipEnded();
        void OnUnequipEnded();
        void StartSpinAttack();
        void EndSpinAttack();
        void AttackStepIn(Vector3 aimPosition);
        void Bounce();
        void Bounce(Vector3 bounceDir);
    }
}