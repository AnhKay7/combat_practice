using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMoveState : PlayerState
{
    public PlayerMoveState(Player _player, PlayerStateMachine _stateMachine) : base(_player, _stateMachine)
    {
    }
    public override void EnterState()
    {
        base.EnterState();
    }
    public override void FrameUpdate()
    {
        base.FrameUpdate();

        if (player.Input.MoveDirection == 0f)
        {
            stateMachine.ChangeState(player.IdleState);
            return;
        }
        if (player.Input.JumpInput)
        {
            stateMachine.ChangeState(player.JumpState);
            return;
        }
    }
    public override void PhysicUpdate()
    {
        base.PhysicUpdate();
        const float EPSILON = 0.001f;
        float velocityX = player.Movement.velocityX;

        bool isTurning = Mathf.Sign(player.Input.MoveDirection) != Mathf.Sign(velocityX)
                    && Mathf.Abs(velocityX) > EPSILON;
        float curAcceleration = isTurning ? player.TurnAccel : player.BaseAccel;
        velocityX += player.Input.MoveDirection * curAcceleration * Time.fixedDeltaTime;
        velocityX = Mathf.Clamp(velocityX, -player.MaxMoveSpeed, player.MaxMoveSpeed);

        player.Movement.SetVelocityX(velocityX);
    }
    public override void ExitState()
    {
        base.ExitState();
    }
}
