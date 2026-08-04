using UnityEngine;

public class PlayerWallSlideState : PlayerState
{
    public PlayerWallSlideState(Player _player, PlayerStateMachine _stateMachine) : base(_player, _stateMachine)
    {
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        if (player.Ground.IsGrounded)
        {
            if (player.Input.MoveDirection != player.Wall.WallDirection)
            {
                stateMachine.ChangeState(player.MoveState);
            }
            else
            {
                stateMachine.ChangeState(player.IdleState);
            }
            return;
        }
        if (player.Input.JumpInput)
        {
            stateMachine.ChangeState(player.WallJumpState);
            return;
        }

        bool isPressingAwayFromWall = player.Input.MoveDirection != 0f && player.Input.MoveDirection != player.Wall.WallDirection;

        if (!player.Wall.IsTouchingWall || isPressingAwayFromWall)
        {
            stateMachine.ChangeState(player.FallState);
            return;
        }
    }
    public override void PhysicUpdate()
    {
        base.PhysicUpdate();
        float velocityY = player.Movement.velocityY;
        if (velocityY < 0f)
        {
            velocityY = Mathf.Max(velocityY, player.WallSlideSpeed);
            player.Movement.SetVelocityY(velocityY);
        }
    }
}
