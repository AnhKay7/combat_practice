using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float x_input;
    [SerializeField] private float move_speed = 8f;
    [SerializeField] private float jump_force = 8f;
    private PlayerControls controls;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        x_input = 0f;
        x_input = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.UpArrow))
            rb.linearVelocity = new Vector2(rb.linearVelocityX, jump_force);

    }
    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(x_input * move_speed, rb.linearVelocity.y);
    }
}
