namespace WhiteKNight
{
    public interface ICameraModel
    {
        CameraMode GetCurrentMode();
        void OnCameraModeChanged();
    }
}

