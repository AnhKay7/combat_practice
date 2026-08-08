using UnityEngine;

public class PlayerGroundState : PlayerMovementState
{
    public PlayerGroundState(Player _player, PlayerStateMachine _stateMachine) : base(_player, _stateMachine)
    {
    }
    public override bool FrameUpdate()
    {
        if (base.FrameUpdate())
            return true;
        if (!player.Ground.IsGrounded)
        {
            stateMachine.ChangeState(player.FallState);
            return true;
        }

        if (player.Input.JumpInput)
        {
            stateMachine.ChangeState(player.JumpState);
            return true;
        }

        return false;
    }
}
