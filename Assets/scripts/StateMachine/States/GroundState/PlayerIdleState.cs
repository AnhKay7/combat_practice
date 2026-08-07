using UnityEngine;

public class PlayerIdleState : PlayerGroundState
{
    public PlayerIdleState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }
    public override bool FrameUpdate()
    {
        if (base.FrameUpdate())
            return true;

        if (player.Input.MoveDirection != 0f)
        {
            stateMachine.ChangeState(player.MoveState);
            return true;
        }

        return false;
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
