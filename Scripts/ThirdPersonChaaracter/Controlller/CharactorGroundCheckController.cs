using Vector3 = UnityEngine.Vector3;
using UnityEngine;

namespace WhiteKNight
{
    public class CharactorGroundCheckController : MonoBehaviour, ICharacterGroundCheckController
    {
        [SerializeField] private Transform _checkOrigin;

        [SerializeField] private Animator _animator;
        [SerializeField] private CapsuleCollider _capsuleCollider;
        [SerializeField] private float _groundCheckDistance = 5f;
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private Rigidbody _rigidBody;

        void Awake()
        {
            _checkOrigin = _checkOrigin == null ? GetComponent<Transform>() : _checkOrigin;
 
            _animator = _animator == null ? GetComponent<Animator>() : _animator;
            _capsuleCollider = _capsuleCollider == null ? GetComponent <CapsuleCollider>() : _capsuleCollider;
        }

        public bool IsGrounded()
        {
            if(_checkOrigin == null) return false; 
            float radius = _capsuleCollider.radius * 0.9f;

            Vector3 origin = _capsuleCollider.bounds.center;

            float distance = _capsuleCollider.bounds.extents.y + 0.05f;

            return Physics.SphereCast(
                origin,
                radius,
                Vector3.down,
                out _,
                distance,
                _groundLayer
            );
        }

        public Vector3 GetGroundNormal()
        {
            return Physics.Raycast(_checkOrigin.position + Vector3.up * 0.1f, Vector3.down, out var hit, _groundCheckDistance)
                ? hit.normal
                : Vector3.up;
        }

        public float GetGroundCheckDistance()
        {
            float radius = _capsuleCollider.radius * 0.95f;

            Vector3 origin = new Vector3(
                _capsuleCollider.bounds.center.x,
                _capsuleCollider.bounds.min.y + radius + 0.01f,
                _capsuleCollider.bounds.center.z
            );

            float distance = 0.1f;

            Physics.SphereCast(
                origin,
                radius,
                Vector3.down,
                out _,
                distance,
                _groundLayer
            );
            return distance;

        }
        public float GetGroundDistance()
        {
            float radius = _capsuleCollider.radius * 0.95f;

            Vector3 origin = new Vector3(
                _capsuleCollider.bounds.center.x,
                _capsuleCollider.bounds.min.y + radius + 0.01f,
                _capsuleCollider.bounds.center.z
            );

            float maxDistance = 5f;

            if (Physics.SphereCast(
                origin,
                radius,
                Vector3.down,
                out RaycastHit hit,
                maxDistance,
                _groundLayer))
            {
                return hit.distance;
            }

            return maxDistance;
        }

        public bool IsAnimatorGroundedState() => _animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded");

        public bool CheckRayGroundHit()
        {
            RaycastHit hitInfo;
            return (Physics.Raycast(_checkOrigin.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, _groundCheckDistance));
        }

        public float VerticalVelocity()
        {
            return _rigidBody.velocity.y;
        }
    }
}

