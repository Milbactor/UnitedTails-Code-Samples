namespace WhiteKNight
{
    public class CameraModel : ICameraModel
    {
        private CameraMode _currentMode = CameraMode.AutoRotate;

        public CameraMode GetCurrentMode()
        {
            return _currentMode;
        }

        public void OnCameraModeChanged()
        {
            if (_currentMode == CameraMode.AutoRotate)
            {
                _currentMode = CameraMode.MouseRotate;
            }
            else
            {
                _currentMode = CameraMode.AutoRotate;
            }
        }
    }
}

