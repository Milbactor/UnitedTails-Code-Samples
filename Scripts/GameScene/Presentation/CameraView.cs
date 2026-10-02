using Cinemachine;
using System.Collections;
using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public class CameraView : MonoBehaviour, ICameraView
    {
        [SerializeField] private Transform mainCamera;
        [SerializeField] private CinemachineVirtualCamera autoCamera;
        [SerializeField] private CinemachineFreeLook freeLookCamera;

        private const int ActivePriority = 20;
        private const int InactivePriority = 10;

        private Transform _followTarget;
        private Transform _deathCameraAnchor;

        private void Awake()
        {
            if (mainCamera == null && Camera.main != null)
            {
                mainCamera = Camera.main.transform;
            }

            var anchorObject = new GameObject("DeathCameraAnchor");
            _deathCameraAnchor = anchorObject.transform;
        }

        public void SetFollowTarget(Transform target)
        {
            _followTarget = target;

            autoCamera.Follow = target;
            autoCamera.LookAt = target;

            freeLookCamera.Follow = target;
            freeLookCamera.LookAt = target;
        }

        public Transform GetCurrentCameraTransform()
        {
            return mainCamera;
        }

        public void SwitchCameraMode(CameraMode cameraMode)
        {
            if (cameraMode == CameraMode.AutoRotate)
            {
                autoCamera.Priority = ActivePriority;
                freeLookCamera.Priority = InactivePriority;
            }
            else
            {
                autoCamera.Priority = InactivePriority;
                freeLookCamera.Priority = ActivePriority;
            }
        }

        public void LockFollowPosition()
        {
            if (_followTarget == null) { return; }

            Observable.Timer(System.TimeSpan.FromSeconds(2.5f))
                .Subscribe(_ =>
                {
                    _deathCameraAnchor.position = _followTarget.position;

                    autoCamera.Follow = _deathCameraAnchor;
                    freeLookCamera.Follow = _deathCameraAnchor;

                    SwitchCameraMode(CameraMode.MouseRotate);
                })
                .AddTo(this);
        }

        private void OnDestroy()
        {
            if (_deathCameraAnchor != null)
            {
                Destroy(_deathCameraAnchor.gameObject);
            }
        }
    }

    public enum CameraMode
    {
        AutoRotate,
        MouseRotate
    }
}