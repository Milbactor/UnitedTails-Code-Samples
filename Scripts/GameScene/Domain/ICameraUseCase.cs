namespace WhiteKNight
{
    public interface ICameraUseCase
    {
        void OnCameraModeChangeRequested();
        CameraMode GetCameraMode();
    }
}

