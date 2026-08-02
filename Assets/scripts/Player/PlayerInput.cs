using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public bool jump_pressed { get; private set; }
    public bool jump_held { get; private set; }
    public bool jump_released { get; private set; }
    public bool dash_pressed { get; private set; }
    public float move_direction { get; private set; }
    public bool is_facing_right { get; private set; }
    private void Awake()
    {
        is_facing_right = true;
    }
    private void GetPlayerInput()
    {
        jump_pressed = Input.GetKeyDown(KeyCode.Z);
        jump_held = Input.GetKey(KeyCode.Z);
        jump_released = Input.GetKeyUp(KeyCode.Z);

        dash_pressed = Input.GetKeyDown(KeyCode.C);

        move_direction = 0f;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            move_direction = -1f;
            is_facing_right = false;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            move_direction = 1f;
            is_facing_right = true;
        }
    }
    private void Update()
    {
        GetPlayerInput();
    }
}
