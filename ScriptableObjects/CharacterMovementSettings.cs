using UnityEngine;

[CreateAssetMenu(
    fileName = "CharacterMovementSettings",
    menuName = "WhiteKnight/Character Movement Settings")]
public class CharacterMovementSettings : ScriptableObject
{
    public float MovingTurnSpeed = 120;
    public float StationaryTurnSpeed = 60;
    public float TurnSmoothTime = 0.1f;
    public float JumpPower = 12f;
    public float SpinJumpPower = 20f;
    public float RunCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
    public float MoveSpeedMultiplier = 1f;
    [Range(1f, 4f)]
    public float GravityMultiplier = 2f;
    public float AirForwardMaxSpeed = 8f;
    public float AirForwardAcceleration = 15f;

    public float AirLateralMaxSpeed = 2f;
    public float LateralAmount = 1f;
    public float MaxFallSpeed = 20f;
}