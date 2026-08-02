using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerStateMachine
{

    public PlayerState current_state { get; private set; }

    public void Initialize(PlayerState starting_state)
    {
        current_state = starting_state;
        current_state.EnterState();
    }

    public void ChangeState(PlayerState new_state)
    {
        current_state.ExitState();
        current_state = new_state;
        current_state.EnterState();
    }
}
