using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private float move_input;
    //[Header("Movement Stats")]
    public float move_speed = 8f;
    private PlayerControls controls;

    private void Awake()
    {
        controls = new PlayerControls();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        move_input = 0f;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.leftArrowKey.isPressed)
                move_input = -1f;
            if (Keyboard.current.rightArrowKey.isPressed)
                move_input = 1f;
        }
        //move_input = controls.Gameplay.Move.ReadValue<float>();
    }
    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(move_input * move_speed, rb.linearVelocity.y);
    }
}
