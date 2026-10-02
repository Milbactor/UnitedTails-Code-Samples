using UnityEngine;

namespace WhiteKNight
{
    public interface IAIMoveCommandUseCase
    {
        void Tick(
            Vector3 ownPosition,
            Vector3 moveTargetPosition);

        AIMoveCommand GetMoveCommand();
    }
}