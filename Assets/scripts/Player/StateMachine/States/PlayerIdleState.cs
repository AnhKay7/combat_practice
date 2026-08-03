using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void EnterState(){

        base.EnterState();

        //player.Movement.SetVelocityX(0f);

    }
    public override void FrameUpdate()
    {
        base.FrameUpdate();

        if (player.Input.MoveDirection != 0f)
        {
            stateMachine.ChangeState(player.MoveState);
            return;
        }

        //if (player.Input.JumpInput)
        //{
        //    player.Input.UseJumpInput();
        //    stateMachine.ChangeState(player.JumpState);
        //}
    }
    public override void PhysicUpdate()
    {
        base.PhysicUpdate();
        float velocityX = player.Movement.velocityX;
        float cur_deceleration = player.Ground.IsGrounded ? player.GroundDecel : player.AirDecel;
        velocityX = Mathf.MoveTowards(velocityX, 0f, cur_deceleration * Time.fixedDeltaTime);

        player.Movement.SetVelocityX(velocityX);
    }
}
