using UnityEngine;

namespace WhiteKNight
{
    public class ItemCameraBillboardView : MonoBehaviour
    {
        private Camera _mainCamera;

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        private void LateUpdate()
        {
            if (_mainCamera == null)
                return;

            transform.rotation = _mainCamera.transform.rotation;
        }
    }
}