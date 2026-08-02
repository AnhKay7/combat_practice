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

    private float jumpBufferCounter;
    private float dashBufferCounter;

    public bool JumpInput => jumpBufferCounter > 0f;
    public bool DashInput => dashBufferCounter > 0f;
    public void GetPlayerInput()
    {
        if (jumpBufferCounter > 0f)
            jumpBufferCounter -= Time.deltaTime;
        if (dashBufferCounter > 0f)
            dashBufferCounter -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Z))
            jumpBufferCounter = jumpBufferTime;
        JumpHeld = Input.GetKey(KeyCode.Z);
        JumpReleased = Input.GetKeyUp(KeyCode.Z);

        if (Input.GetKeyDown(KeyCode.C))
            dashBufferCounter = dashBufferTime;

        MoveDirection = 0f;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            MoveDirection = -1f;
            IsFacingRight = false;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            MoveDirection = 1f;
            IsFacingRight = true;
        }
    }
    public void UseJumpInput() => jumpBufferCounter = 0f;
    public void UseDashInput() => dashBufferCounter = 0f;
}
