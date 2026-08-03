using UnityEngine;

public abstract class PlayerState
{
    protected Player player;
    protected PlayerStateMachine stateMachine;
    //protected string animation_name;

    public PlayerState(Player _player, PlayerStateMachine _stateMachine)
    {
        this.player = _player;
        this.stateMachine = _stateMachine;
    }

    public virtual void EnterState()
    {

    }

    public virtual void ExitState()
    {

    }

    public virtual void FrameUpdate()
    {

    }

    public virtual void PhysicUpdate()
    {
        float velocityY = player.Movement.velocityY;
        float gravity = Physics2D.gravity.y * player.GravityScale;

        if (velocityY < 0)
        {
            gravity *= player.FallGravityMultiplier;
        }

        velocityY += gravity * Time.fixedDeltaTime;
        velocityY = Mathf.Max(velocityY, player.MaxFallSpeed);

        player.Movement.SetVelocityY(velocityY);
    }
}
