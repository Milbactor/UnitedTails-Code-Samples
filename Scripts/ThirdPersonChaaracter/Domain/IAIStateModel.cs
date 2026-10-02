using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public interface IAIStateModel
    {
        public AIMovementMode CurrentMode { get; set; }
        ReactiveProperty<bool> RequestJump { get; }
        ReactiveProperty<bool> RequestSpinJump { get; }
        ReactiveProperty<bool> RequestAttack { get; set; }
        ReactiveProperty<bool> RequestSpinAttack { get; set; }
        ReactiveProperty<bool> RequestMove { get; set; }
        ReactiveProperty<bool> RequestEquip { get; set; }
        ReactiveProperty<bool> RequestUnequip { get; set; }

        AIMoveState CurrentMoveState { get;  }
        Vector3 MoveDirection { get; set; }

        void SetMoveState(AIMoveState state);
    }
}