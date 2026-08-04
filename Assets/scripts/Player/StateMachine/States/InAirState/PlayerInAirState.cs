using UnityEngine;
using UnityEngine.EventSystems;

public abstract class PlayerInAirState : PlayerState
{
    protected float wallJumpInputUnlockTime = -100f;
    public PlayerInAirState(Player _player, PlayerStateMachine _stateMachine) : base(_player, _stateMachine)
    {
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
    }
    public override void PhysicUpdate()
    {
        base.PhysicUpdate();

        float velocityX = player.Movement.velocityX;
        float moveDirection = player.Input.MoveDirection;

        float curAirControl = 1f;
        if (wallJumpInputUnlockTime > Time.time)
        {
            curAirControl = player.WallJumpAirControl;
        }
        if (moveDirection != 0)
        {
            bool isTurning = Mathf.Sign(moveDirection) != Mathf.Sign(velocityX) && Mathf.Abs(velocityX) > 0.001f;
            float currentAccel = isTurning ? player.TurnAccel : player.BaseAccel;
            if (Mathf.Abs(velocityX) > player.MaxMoveSpeed) {
                
                if (Mathf.Sign(moveDirection) == Mathf.Sign(velocityX))
                {
                    velocityX = Mathf.MoveTowards(velocityX, Mathf.Sign(velocityX) * player.MaxMoveSpeed, player.AirDecel * Time.fixedDeltaTime);
                }
                else
                {
                    velocityX += moveDirection * currentAccel * curAirControl * Time.fixedDeltaTime;
                }
            }
            else
            {
                velocityX += moveDirection * currentAccel * curAirControl * Time.fixedDeltaTime;
                velocityX = Mathf.Clamp(velocityX, -player.MaxMoveSpeed, player.MaxMoveSpeed);
            }
        }
        else
        {
            velocityX = Mathf.MoveTowards(velocityX, 0f, player.AirDecel * Time.fixedDeltaTime);
        }
        player.Movement.SetVelocityX(velocityX);
    }
}
