using UnityEngine;

public abstract class PlayerState
{
    protected Player player;
    protected PlayerStateMachine state_machine;
    //protected string animation_name;

    public PlayerState(Player player, PlayerStateMachine state_machine)
    {
        this.player = player;
        this.state_machine = state_machine;
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

    }
}
