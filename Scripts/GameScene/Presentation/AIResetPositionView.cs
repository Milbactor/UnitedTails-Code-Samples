using UnityEngine;
using UnityEngine.AI;

namespace WhiteKNight
{
    public class AIResetPositionView : MonoBehaviour
    {
        [SerializeField] private float fallResetHeight = -10;

        [SerializeField] private float behindCameraDistance = 3f;

        [SerializeField] private float navMeshSearchRadius = 8f;

        [SerializeField] private Rigidbody _rigidbody;

        [SerializeField] private NavMeshAgent _navMeshAgent;

        [SerializeField] private Transform _player;

        public bool IsBelowFallResetHeight =>
            transform?.position.y < fallResetHeight;

        public bool TryGetResetPosition(
            Vector3 fallbackSpawnPosition,
            out Vector3 resetPosition)
        {
            if (TryGetPositionBehindCamera(
                    out resetPosition))
            {
                return true;
            }

            if (_player != null &&
                TryGetNavMeshPosition(
                    _player.position,
                    out resetPosition))
            {
                return true;
            }

            if (TryGetNavMeshPosition(
                    fallbackSpawnPosition,
                    out resetPosition))
            {
                return true;
            }

            // SpawnPositionBは安全な場所へ
            // 配置済みという前提の最終手段
            resetPosition = fallbackSpawnPosition;
            return true;
        }

        private bool TryGetPositionBehindCamera(
            out Vector3 resetPosition)
        {
            Camera mainCamera = Camera.main;

            if (mainCamera == null)
            {
                resetPosition = default;
                return false;
            }

            Vector3 flatForward =
                Vector3.ProjectOnPlane(
                    mainCamera.transform.forward,
                    Vector3.up)
                .normalized;

            if (flatForward.sqrMagnitude <= 0.001f)
            {
                resetPosition = default;
                return false;
            }

            Vector3 desiredPosition =
                mainCamera.transform.position -
                flatForward * behindCameraDistance;

            return TryGetNavMeshPosition(
                desiredPosition,
                out resetPosition);
        }

        private bool TryGetNavMeshPosition(
            Vector3 desiredPosition,
            out Vector3 navMeshPosition)
        {
            if (NavMesh.SamplePosition(
                    desiredPosition,
                    out NavMeshHit hit,
                    navMeshSearchRadius,
                    NavMesh.AllAreas))
            {
                navMeshPosition = hit.position;
                return true;
            }

            navMeshPosition = default;
            return false;
        }

        public void ResetPosition(Vector3 resetPosition)
        {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;

            /*
             * 現在地がNavMesh外の場合、Agentがenabledでも
             * isOnNavMeshはfalseになり得る。
             *
             * 一度無効化して、有効なNavMesh座標へ移してから
             * Agentを再度有効化する。
             */
            _navMeshAgent.enabled = false;

            transform.position = resetPosition;

            Physics.SyncTransforms();

            _navMeshAgent.enabled = true;
        }
    }
}