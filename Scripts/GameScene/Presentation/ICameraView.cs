using UnityEngine;
using static WhiteKNight.CameraUseCase;

namespace WhiteKNight
{
    public interface ICameraView
    {
        void SetFollowTarget(Transform target);
        Transform GetCurrentCameraTransform();
        void SwitchCameraMode(CameraMode cameraMode);
        void LockFollowPosition();
    }
}