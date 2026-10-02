using UnityEngine;

namespace WhiteKNight
{
    public class ThirdPersonCharacterInput: MonoBehaviour, IThirdPersonCharacterInput
    {
        private Vector2 _move = new Vector2();
        public Vector2 MoveInput => _move;

        private float _mouseX;
        public float MouseX => _mouseX;

        private bool _jumpPressed = false;
        public bool JumpPressed => _jumpPressed;

        private bool jump;

        private bool _attackPressed = false;
        public bool AttackPressed => _attackPressed;

        private bool _unequipPressed = false;
        public bool UnequipPressed => _unequipPressed;

        private Transform _target;
        public Transform Target => _target;

        private bool attack;
        private bool unequip;
  
        private void Update()
        {
            if (!jump)
                _jumpPressed = Input.GetButtonDown("Jump");
            if (!attack)
                _attackPressed = Input.GetMouseButtonDown(0);
            if (!unequip)
                _unequipPressed = Input.GetMouseButtonDown(1);
        }

        private void FixedUpdate()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Mathf.Max(0f, Input.GetAxis("Vertical"));

            _move = new Vector2(horizontal, vertical);

            _mouseX = Input.GetAxis("Mouse X");

            jump = false;
            attack = false;
            unequip = false;
        }

        public void SetTarget(Transform targetTransform)
        { 
            this._target = targetTransform;
        }

        public bool TryClosestTargetPoint(out Vector3 closestPoint)
        {
            closestPoint = default;
            if(_target == null) return false;

            var _targetCollider = _target.GetComponent<IEnemyView>()?.CombatCollider;
            if (_targetCollider == null)
            {
                closestPoint =  _target.position;
                return true;
            }

            closestPoint = _targetCollider.ClosestPoint(transform.position);
            return true;
        }

        public void SetInputEnabled(bool enabled)
        {
           
        }
    }
}