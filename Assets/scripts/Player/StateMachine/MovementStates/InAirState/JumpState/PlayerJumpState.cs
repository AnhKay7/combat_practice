using UnityEngine;

public class PlayerJumpState : PlayerInAirState
{
    public PlayerJumpState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        player.ConsumeAllJumpGraces();
        ExecuteJump();
    }

    private void ExecuteJump()
    {
        float targetVelocityY = Mathf.Sqrt(player.JumpHeight * -2 * (Physics2D.gravity.y * player.GravityScale));
        if (!player.Input.JumpHeld)
        {
            targetVelocityY *= player.JumpCutMultiplier;
        }
        player.Movement.SetVelocityY(targetVelocityY);
    }
    private void CutJump()
    {
        float velocityY = player.Movement.velocityY;

        velocityY *= player.JumpCutMultiplier;

        player.Movement.SetVelocityY(velocityY);
    }

    public override bool FrameUpdate()
    {
        if (base.FrameUpdate())
            return true;
        if (player.Input.JumpReleased && player.Movement.velocityY > 0)
        {
            CutJump();
        }
        if (player.Movement.velocityY <= 0f)
        {
            stateMachine.ChangeState(player.FallState);
            return true;
        }
        return false;
    }
}