using UnityEngine;
using WhiteKNight;

public class DeathAnimationBehaviour :  StateMachineBehaviour 
{
    [SerializeField] private SoundEffectId _soundEffectId;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SoundManager.Instance?.PlaySE(_soundEffectId);
    }
}