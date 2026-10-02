namespace WhiteKNight
{
    public class CameraPresenter : ICameraPresenter
    {
        private CameraView _cameraView;
        private ICameraUseCase _cameraUseCase;

        public CameraPresenter(CameraView cameraView, ICameraUseCase cameraUseCase)
        {
            _cameraView = cameraView;
            _cameraUseCase = cameraUseCase;
        }

        public void Initialize()
        {
            _cameraView.SwitchCameraMode(_cameraUseCase.GetCameraMode());
        }

        public void OnPlayerDead()
        {
            _cameraView.LockFollowPosition();
        }

        public void OnSwitchCamera()
        {
            _cameraUseCase.OnCameraModeChangeRequested();
            var cameraMode = _cameraUseCase.GetCameraMode();
            _cameraView.SwitchCameraMode(cameraMode);
        }
    }
}