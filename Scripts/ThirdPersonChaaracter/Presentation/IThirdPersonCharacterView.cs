using System;
using UniRx;
using UnityEngine;

namespace WhiteKNight
{
    public interface IThirdPersonCharacterView
    {
        public IObservable<Unit> OnUpdate { get; }
        public IObservable<Unit> OnJumpFinished { get; }
        public IObservable<Unit> OnSpinJumpFinished{ get; }
       
        //void ScaleCapsuleForCrouching(float height, Vector3 center);
        
        void ApplyExtraTurnRotation();
        void UpdateAnimator(Vector3 move, bool isGrounded);
        void SetApplyRootMotion(bool isRootMotion);
        void SetGroundState(bool grounded);
        Vector3 TransformDirection(Vector3 localDirection);
        void StartJump();
        void JumpFinished();
        void StartSpinJump();
        void SpinJumpFinished();
    }
}