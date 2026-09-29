using UnityEngine;

namespace WhiteKNight
{
    public struct AIMoveCommand
    {
        public AIMovementMode movementMode;
        public Vector3 Direction;

        public AIMoveCommand(AIMovementMode movementMode, Vector3 Direction)
        {
            this.movementMode = movementMode;
            this.Direction = Direction;
        }
    }
}