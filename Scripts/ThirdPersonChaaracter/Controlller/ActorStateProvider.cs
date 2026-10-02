using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public class ActorStateProvider : MonoBehaviour, IActorStateProvider
    {
        public Vector3 Position => transform.position;

        public IReactiveProperty<bool> IsJumping => _isJumping;
        private ReactiveProperty<bool> _isJumping = new(false);

        public IReactiveProperty<bool> IsSpinJumping => _isSpinJumping;
        private ReactiveProperty<bool> _isSpinJumping = new ReactiveProperty<bool>(false);

        public IReactiveProperty<bool> IsGround => _isGround;
        private ReactiveProperty<bool> _isGround = new ReactiveProperty<bool>(false);
    }
}