using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public class AIStateModel : IAIStateModel
    {
        public AIMovementMode CurrentMode
        {
            get => _currentMode;
            set { _currentMode = value; }
        }

        public ReactiveProperty<bool> RequestMove { get; set; } = new(false);
        public ReactiveProperty<bool> RequestJump { get; set; } = new(false);
        public ReactiveProperty<bool> RequestSpinJump { get; set; } = new(false);
        public ReactiveProperty<bool> RequestAttack{ get; set; } = new(false);
        public ReactiveProperty<bool> RequestSpinAttack { get; set; } = new(false);
        public ReactiveProperty<bool> RequestEquip { get; set; } = new(false);
        public ReactiveProperty<bool> RequestUnequip { get; set; } = new(false);
        
        public Vector3 MoveDirection
        {
            get => _moveDirection;
            set => _moveDirection = value;
        }

        private AIMoveState _moveState = AIMoveState.ChasingPartner;
        public AIMoveState CurrentMoveState => _moveState;

        private AIMovementMode _currentMode = AIMovementMode.NavMesh;
        private Vector3 _moveDirection;

        public void SetMoveState(AIMoveState state)
        {
            _moveState = state;
        }
    }

    public enum AIMovementMode
    {
        NavMesh,
        Physics
    }

    public enum AIMoveState
    {
        ChasingEnemy,
        Attacking, 
        ChasingPartner,
        Dead
    }

    public static class AICombatDefinition
    {
        public const float UnequipDelay = 2.0f;
    }
}