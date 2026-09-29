using UnityEngine;
using WhiteKNight;

public class SpinJumpAnimBehaviour : StateMachineBehaviour
{
    [SerializeField] private SoundEffectId _soundEffectId;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //var attackLayer = animator.GetLayerIndex("Attack Layer");
        //animator.SetLayerWeight(attackLayer, 0f);
        //　avatar mask used to make knight hold sword required so probably no need to set 0 (more test needed)

        SoundManager.Instance?.PlaySE(_soundEffectId);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //var attackLayer = animator.GetLayerIndex("Attack Layer");
       // animator.SetLayerWeight(attackLayer, 1f);
        animator.gameObject.GetComponent<IThirdPersonCharacterView>().SpinJumpFinished();
    }
}
