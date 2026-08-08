using UnityEngine;
using UnityEngine.Rendering;

public abstract class PlayerState
{
    protected Player player;
    protected PlayerStateMachine stateMachine;
    //protected string animation_name;
    public virtual bool CanAttack => true;
    public PlayerState(Player _player, PlayerStateMachine _stateMachine)
    {
        this.player = _player;
        this.stateMachine = _stateMachine;
    }

    public virtual void EnterState()
    {
        //Debug.Log("hello from " + stateMachine.CurrentState.ToString());
    }

    public virtual void ExitState()
    {

    }

    public virtual bool FrameUpdate()
    {
        return false;
    }

    public virtual void PhysicUpdate()
    {
    }
}
