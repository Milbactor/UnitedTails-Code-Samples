using Cinemachine;
using UnityEngine;

namespace WhiteKnight
{
    public class CameraVerticalOrbit : MonoBehaviour
    {
        [SerializeField]
        private CinemachineVirtualCamera virtualCamera;

        [SerializeField]
        private float initialVerticalOffset = 2f;

        [SerializeField]
        private float verticalSpeed = 4f;

        [SerializeField]
        private float minVerticalOffset = 0f;

        [SerializeField]
        private float maxVerticalOffset = 7f;

        [SerializeField]
        private bool invertVertical = false;

        private CinemachineComposer _composer;
        private Vector3 _baseTrackedOffset;
        private float _currentVerticalOffset;

        private void Awake()
        {
            if (virtualCamera == null)
            {
                Debug.LogError(
                    "CameraVerticalOrbit: Virtual Camera is not assigned.",
                    this);

                enabled = false;
                return;
            }

            _composer =
                virtualCamera.GetCinemachineComponent<
                    CinemachineComposer>();

            if (_composer == null)
            {
                Debug.LogError(
                    "CameraVerticalOrbit: Aim must be Composer.",
                    this);

                enabled = false;
                return;
            }

            _baseTrackedOffset =
                _composer.m_TrackedObjectOffset;

            _currentVerticalOffset = Mathf.Clamp(
                initialVerticalOffset,
                minVerticalOffset,
                maxVerticalOffset);

            ApplyVerticalOffset();
        }

        private void LateUpdate()
        {
            float input = Input.GetAxis("Mouse Y");

            if (invertVertical)
                input = -input;

            _currentVerticalOffset +=
                input * verticalSpeed * Time.deltaTime;

            _currentVerticalOffset = Mathf.Clamp(
                _currentVerticalOffset,
                minVerticalOffset,
                maxVerticalOffset);

            Vector3 trackedOffset = _baseTrackedOffset;

            trackedOffset.y += _currentVerticalOffset;

            _composer.m_TrackedObjectOffset =
                trackedOffset;
        }

        private void ApplyVerticalOffset()
        {
            Vector3 trackedOffset = _baseTrackedOffset;

            trackedOffset.y += _currentVerticalOffset;

            _composer.m_TrackedObjectOffset =
                trackedOffset;
        }
    }
}