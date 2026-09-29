using UnityEngine;

namespace WhiteKNight
{
    public class SwordIKView : MonoBehaviour, ICharacterLifeCycle
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _leftHandSocket;

        private bool _useIK = false;
        public bool UseIK { set => _useIK = value; }


        private bool _isDead = false;
        public bool IsDead => _isDead;

        public void OnDead()
        {
            if (_isDead) return;
            _isDead = true;
            _useIK = false;
            _animator.SetLayerWeight(1, 0);
            _animator.ResetTrigger("TriggerAttack");
            _animator.SetBool("IsSpinAttack",false);
        }

        private void Awake()
        {
            if (!TryGetComponent<Animator>(out _animator))
            {
                Debug.LogError("Animator is not assigned ");
                this.enabled = false;
                return;
            }
            if(_leftHandSocket == null)
            {
                Debug.LogError("socket for both hand sword is not assigned "); 
                this.enabled = false;
                return;
            }
        }

        void OnAnimatorIK(int layerIndex)
        {
            if (!_useIK) return;

            _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
            _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);

            _animator.SetIKPosition(
                AvatarIKGoal.LeftHand,
                _leftHandSocket.position
            );

            _animator.SetIKRotation(
                AvatarIKGoal.LeftHand,
                _leftHandSocket.rotation
            );
        }
    }
}

