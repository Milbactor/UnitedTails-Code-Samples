namespace WhiteKNight
{
    public class CameraUseCase : ICameraUseCase
    {      
        private ICameraModel _cameraModel = null;

        public CameraUseCase(ICameraModel cameraModel)
        {
            _cameraModel = cameraModel;
        }

        public void OnCameraModeChangeRequested()
        {
            _cameraModel.OnCameraModeChanged();
        }

        public CameraMode GetCameraMode()
        {
            return _cameraModel.GetCurrentMode();
        }
    }
}

