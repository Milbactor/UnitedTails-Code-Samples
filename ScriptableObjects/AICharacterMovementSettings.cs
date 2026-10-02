using UnityEngine;

[CreateAssetMenu(
    fileName = "AICharacterMovementSettings",
    menuName = "WhiteKnight/AI Character Movement Settings")]

public class AICharacterMovementSettings : ScriptableObject
{
    public float MovingTurnSpeed = 360f;
    public float StationaryTurnSpeed = 180f;
    public float JumpPower = 12f;
    public float SpinJumpPower = 5f;
    public float ApproachSpinJumpPower = 8f;
    public float ApproachJumpPower = 8f;
    [Range(1f, 4f)]
    public float GravityMultiplier = 2f;
    public float RunCycleLegOffset = 0.2f;
    public float MaxFallSpeedForAnim = 8f;

    [Header("Chase")]
    public float MinChaseDistance = 3.1f;
    public float MaxChaseDistance = 6f;
    public float MaxChaseSpeed = 10f;
    public float RotationSmooth = 10f;
}