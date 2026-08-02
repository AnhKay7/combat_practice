using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(Player player, PlayerStateMachine state_machine) : base(player, state_machine)
    {
    }

    public override void EnterState(){

        base.EnterState();

        //player.velocity_x = 0f;
    }
}
