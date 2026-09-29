using UnityEngine;

namespace WhiteKNight
{
    public class OffScreenIndicatorView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform target;
        [SerializeField] private RectTransform indicatorRoot;
        [SerializeField] private RectTransform rotatingFrameAndArrow;
        [SerializeField] private RectTransform canvasRect;

        [Header("Settings")]
        [SerializeField] private float screenEdgePadding = 70f;

        private Camera _mainCamera;
        private Vector2 _lastValidDirection = Vector2.down;
        private bool _isDead = false;

        private void Awake()
        {
            _mainCamera = Camera.main;
            indicatorRoot.gameObject.SetActive(false);
        }

        private void LateUpdate()
        {
            if (target == null || _mainCamera == null || _isDead)
                return;

            Vector3 viewportPosition =
                _mainCamera.WorldToViewportPoint(target.position);

            bool isBehindCamera = viewportPosition.z < 0f;

            bool isOnScreen =
                !isBehindCamera &&
                viewportPosition.x >= 0f &&
                viewportPosition.x <= 1f &&
                viewportPosition.y >= 0f &&
                viewportPosition.y <= 1f;

            if (isOnScreen)
            {
                indicatorRoot.gameObject.SetActive(false);
                return;
            }

            UpdateIndicatorPosition(
                viewportPosition,
                isBehindCamera);

            indicatorRoot.gameObject.SetActive(true);
        }

        private void UpdateIndicatorPosition(
            Vector3 viewportPosition,
            bool isBehindCamera)
        {
            Vector2 direction = new(
                viewportPosition.x - 0.5f,
                viewportPosition.y - 0.5f);

            if (isBehindCamera)
                direction = -direction;

            if (direction.sqrMagnitude < 0.0001f)
            {
                direction = _lastValidDirection;
            }
            else
            {
                direction.Normalize();
                _lastValidDirection = direction;
            }

            float halfWidth =
                canvasRect.rect.width * 0.5f - screenEdgePadding;

            float halfHeight =
                canvasRect.rect.height * 0.5f - screenEdgePadding;

            float distanceToVerticalEdge =
                Mathf.Abs(direction.x) > 0.001f
                    ? halfWidth / Mathf.Abs(direction.x)
                    : float.MaxValue;

            float distanceToHorizontalEdge =
                Mathf.Abs(direction.y) > 0.001f
                    ? halfHeight / Mathf.Abs(direction.y)
                    : float.MaxValue;

            float edgeDistance =
                Mathf.Min(
                    distanceToVerticalEdge,
                    distanceToHorizontalEdge);

            indicatorRoot.anchoredPosition =
                direction * edgeDistance;

            float angle =
                Mathf.Atan2(direction.y, direction.x)
                * Mathf.Rad2Deg
                - 90f;

            rotatingFrameAndArrow.localRotation =
                Quaternion.Euler(0f, 0f, angle);
        }

        public void SetIndicatorEnabled(bool isEnabled)
        {
            indicatorRoot.gameObject.SetActive(isEnabled);
        }

        public void HideIndicator()
        {
            indicatorRoot.gameObject.SetActive(false);
        }

        public void OnDead()
        {
            _isDead = true;
            HideIndicator();
        }
    }
}