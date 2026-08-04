using UnityEngine;

public class PlayerFallState : PlayerInAirState
{
    public PlayerFallState(Player _player, PlayerStateMachine _stateMachine) : base(_player, _stateMachine)
    {
    }
    public override void FrameUpdate()
    {
        base.FrameUpdate();

        if (player.Input.JumpInput)
        {
            if (Time.time - player.LastTimeGrounded < player.CoyoteTime)
            {
                stateMachine.ChangeState(player.JumpState);
                return;
            }
            if (Time.time - player.LastTimeOnWall < player.WallCoyoteTime)
            {
                stateMachine.ChangeState(player.WallJumpState);
                return;
            }
        }
        if (player.Ground.IsGrounded && player.Movement.velocityY <= 0f)
        {
            if (player.Input.MoveDirection != 0f)
            {
                stateMachine.ChangeState(player.MoveState);
            }
            else
                stateMachine.ChangeState(player.IdleState);
            return;
        }
        if (player.Wall.IsTouchingWall)
        {
            stateMachine.ChangeState(player.WallSlideState);
            return;
        }
    }
}
