using Unity.VisualScripting;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public bool JumpHeld { get; private set; }
    public bool JumpReleased { get; private set; }
    public float MoveDirection { get; private set; }
    public bool IsFacingRight { get; private set; } = true;

    [Header("Input Buffer Settings")]
    [SerializeField] private float jumpBufferTime = 0.15f;
    [SerializeField] private float dashBufferTime = 0.15f;

    private float lastTimePressedJump;
    private float lastTimePressedDash;

    public bool JumpInput => Time.time < lastTimePressedJump + jumpBufferTime;
    public bool DashInput => Time.time < lastTimePressedDash + dashBufferTime;
    public bool AttackInput => Input.GetKeyDown(KeyCode.X);
    public void GetPlayerInput()
    {

        if (Input.GetKeyDown(KeyCode.Z))
            lastTimePressedJump = Time.time;
        JumpHeld = Input.GetKey(KeyCode.Z);
        JumpReleased = Input.GetKeyUp(KeyCode.Z);

        if (Input.GetKeyDown(KeyCode.C))
            lastTimePressedDash = Time.time;

        MoveDirection = GetPlayerMoveDirection();
        if (MoveDirection != 0f)
        {
            IsFacingRight = MoveDirection > 0f;
        }
    }
    private float GetPlayerMoveDirection()
    {
        float rightValue = Input.GetKey(KeyCode.RightArrow) ? 1f : 0f;
        float leftValue = Input.GetKey(KeyCode.LeftArrow) ? 1f : 0f;

        return rightValue - leftValue;
    }
    public void UseJumpInput() => lastTimePressedJump = -100f;
    public void UseDashInput() => lastTimePressedDash = -100f;
}
