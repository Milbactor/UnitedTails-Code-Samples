using UnityEngine;
using WhiteKNight;

public class SpinAttackAnimationBehaviour : StateMachineBehaviour
{
    private IThirdPersonCharacterAttackView _thirdPersonCharacterAttackView = null;
    [SerializeField] private SoundEffectId _soundEffectId;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetLayerWeight(1, 0);
        SoundManager.Instance?.PlaySE(_soundEffectId);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_thirdPersonCharacterAttackView == null)
        {
            _thirdPersonCharacterAttackView =
                animator.GetComponentInParent<IThirdPersonCharacterAttackView>();
        }
        animator.SetLayerWeight(1, 1);
        _thirdPersonCharacterAttackView?.EndSpinAttack();

    }
}
