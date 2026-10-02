using UnityEngine;


public class MatchPositionBehavior : StateMachineBehaviour
{
    public IMatchTarget target;

    [Header("Match Settings")]
    [SerializeField] AvatarTarget targetBodyPart = AvatarTarget.Root;
    [SerializeField, Range(0,1)] Vector2 effectRange;

    [Header("Assist Settings")]
    [SerializeField, Range(0, 1)] float assistPower = 1f;
    [SerializeField, Range(0, 10)] float assistDistance = 1f;

    MatchTargetWeightMask weightmask;
    bool isSkip = false;
    bool isInitialized = false;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(isInitialized == false)
        {
            var weight = new Vector3(assistPower, 0, assistPower);
            weightmask = new MatchTargetWeightMask(weight, 0);
            isInitialized = true;
        }

        if (target == null)
            return;

        isSkip = Vector3.Distance(target.TargetPosition, animator.rootPosition) > assistDistance;
        
    }

    public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (isSkip == true || animator.IsInTransition(layerIndex))
            return;

        if(stateInfo.normalizedTime > effectRange.y)
        {
            animator.InterruptMatchTarget(false);
        }
        else
        {
            animator.MatchTarget(
                target.TargetPosition,
                animator.bodyRotation,
                targetBodyPart,
                weightmask,
                effectRange.x, effectRange.y);
        }
    }
}

public interface IMatchTarget
{
    Vector3 TargetPosition { get; }
}


