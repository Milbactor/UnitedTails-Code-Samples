using UnityEngine;
using WhiteKNight;

public class AttackAnimationBehaviour : StateMachineBehaviour
{
    private IThirdPersonCharacterAttackView _thirdPersonCharacterAttackView = null;
    [SerializeField] private SoundEffectId _soundEffectId;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       if (_thirdPersonCharacterAttackView == null)
       {
           _thirdPersonCharacterAttackView =
                animator.GetComponentInParent<IThirdPersonCharacterAttackView>();
        }
        //_thirdPersonCharacterAttackView?.SetAttackLayerWeight();
        SoundManager.Instance?.PlaySE(_soundEffectId);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
     //   _thirdPersonCharacterAttackView?.ResetAttackLayerWeight();
        _thirdPersonCharacterAttackView.OnAttackHitEnd();
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
