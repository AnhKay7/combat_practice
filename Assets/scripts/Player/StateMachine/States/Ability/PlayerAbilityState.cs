using UnityEngine;

public class PlayerAbilityState : PlayerState
{
    protected float abilityEndTime = -100f;
    public PlayerAbilityState(Player _player, PlayerStateMachine _stateMachine) : base(_player, _stateMachine)
    {
    }
    public override void EnterState()
    {
        base.EnterState();
        abilityEndTime = -100f;
        player.ConsumeAllJumpGraces();
    }
}
