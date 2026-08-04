using UnityEngine;

public class PlayerDashState : PlayerAbilityState
{
    protected override bool AllowDash => false;

    private float dashDirection;
    public PlayerDashState(Player _player, PlayerStateMachine _stateMachine) : base(_player, _stateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        player.Input.UseDashInput();
            
        if (player.Wall.IsTouchingWall)
        {
            dashDirection = -player.Wall.WallDirection;
        }   
        else if (player.Input.MoveDirection != 0f)
        {
            dashDirection = player.Input.MoveDirection;
        }
        else
        {
            dashDirection = player.Input.IsFacingRight ? 1f : -1f;
        }

        player.DashController.ConsumeDash(player.Ground.IsGrounded);
        abilityEndTime = Time.time + player.DashController.DashDuration;
    }
    public override void FrameUpdate()
    {
        base.FrameUpdate();
        if (Time.time >= abilityEndTime)
        {
            if (player.Ground.IsGrounded)
            {
                if (player.Input.MoveDirection != 0)
                    stateMachine.ChangeState(player.MoveState);
                else
                    stateMachine.ChangeState(player.IdleState);
                return;
            }

            if (player.Wall.IsTouchingWall)
            {
                stateMachine.ChangeState(player.WallSlideState);
                return;
            }
            stateMachine.ChangeState(player.FallState);
            return;
        }
    }
    public override void PhysicUpdate()
    {
        base.PhysicUpdate();
        player.Movement.SetVelocityY(0f);
        player.Movement.SetVelocityX(dashDirection * player.DashController.DashSpeed);
    }
}
