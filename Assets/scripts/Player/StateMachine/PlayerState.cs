using UnityEngine;
using UnityEngine.Rendering;

public abstract class PlayerState
{
    protected Player player;
    protected PlayerStateMachine stateMachine;
    //protected string animation_name;
    protected virtual bool AllowDash => true;
    protected bool CheckDashTransition()
    {
        if (AllowDash && player.Input.DashInput && player.DashController.CanDash(player.Ground.IsGrounded))
        {
            stateMachine.ChangeState(player.DashState);
            return true;
        }
        return false;
    }
    public PlayerState(Player _player, PlayerStateMachine _stateMachine)
    {
        this.player = _player;
        this.stateMachine = _stateMachine;
    }

    public virtual void EnterState()
    {
        Debug.Log("hello from " + stateMachine.CurrentState.ToString());
    }

    public virtual void ExitState()
    {

    }

    public virtual void FrameUpdate()
    {
        if (CheckDashTransition())
        {
            return;
        }    
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
