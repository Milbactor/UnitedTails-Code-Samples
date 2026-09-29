using UnityEngine;

namespace WhiteKNight
{
    public class CameraSwitchInputController : MonoBehaviour
    {
        private ICameraPresenter _cameraPresenter;

        public void Initialize(ICameraPresenter cameraPresenter)
        {
            _cameraPresenter = cameraPresenter;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            if (_cameraPresenter == null) return;
            if (Input.GetKeyDown(KeyCode.C) == true)
            {
                _cameraPresenter.OnSwitchCamera();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}