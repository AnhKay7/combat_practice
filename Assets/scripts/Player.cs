using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Object")]
    [SerializeField] private float velocity_y = 0f;

    [Header("Movement")]
    [SerializeField] private float move_direction;
    [SerializeField] private float move_speed = 8f;
    private bool is_facing_right = true;

    [Header("Jump")]
    [SerializeField] private float jump_height = 5f;
    [SerializeField] private float jump_force;
    [SerializeField] private float gravity_scale = 5f;
    [SerializeField] private float max_fall_speed = -20f;
    private GroundSensor ground_sensor;

    [Header("Dash")]
    private bool can_dash = true;

    void Start()
    {
        ground_sensor = GetComponent<GroundSensor>();
    }

    void Update()
    {
        velocity_y += Physics2D.gravity.y * gravity_scale * Time.deltaTime;
        if (ground_sensor.is_ground == true)
        {
            if (velocity_y < 0)
            {
                velocity_y = 0f;
            }
        }
        if (Input.GetKeyDown(KeyCode.Z) && ground_sensor.is_ground)
        {
            jump_force = Mathf.Sqrt(jump_height * -2 * (Physics2D.gravity.y * gravity_scale));
            velocity_y = jump_force;
        }
        velocity_y = Mathf.Max(velocity_y, max_fall_speed);

        transform.Translate(new Vector3(0, velocity_y, 0) * Time.deltaTime);
    }
    void FixedUpdate()
    {
       
    }
}
