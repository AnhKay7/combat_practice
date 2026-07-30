using Unity.VisualScripting;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class Player : MonoBehaviour
{
    [Header("Object")]
    [SerializeField] private float velocity_y = 0f;

    [Header("Movement")]
    private float move_direction;
    private bool is_facing_right = true;
    [SerializeField] private float move_speed = 8f;

    [Header("Jump")]
    [SerializeField] private float jump_height = 5f;
    [SerializeField] private float jump_force;
    [SerializeField] private float gravity_scale = 5f;
    [SerializeField] private float max_fall_speed = -20f;
    private GroundSensor ground_sensor;

    [Header("Dash")]
    private bool can_dash = true;

    [Header("Collision Prediction")]
    [SerializeField] private LayerMask ground_layer;
    private Collider2D col;

    void Start()
    {
        ground_sensor = GetComponent<GroundSensor>();
        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        GetPlayerInput();

        CaculateVerticalVelocity();

        ApplyMovement();
    }
    void FixedUpdate()
    {

    }

    private void GetPlayerInput()
    {
        move_direction = 0f;
        if (Input.GetKey(KeyCode.LeftArrow))
            move_direction = -1f;
        if (Input.GetKey(KeyCode.RightArrow))
            move_direction = 1f;

        if (Input.GetKeyDown(KeyCode.Z) && ground_sensor.is_ground)
        {
            velocity_y = Mathf.Sqrt(jump_height * -2 * (Physics2D.gravity.y * gravity_scale));
        }
    }

    private void CaculateVerticalVelocity()
    {

        velocity_y += Physics2D.gravity.y * gravity_scale * Time.deltaTime;

        velocity_y = Mathf.Max(velocity_y, max_fall_speed);
    }

    private void ApplyMovement()
    {
        float delta_x = move_direction * move_speed * Time.deltaTime;
        float delta_y = velocity_y * Time.deltaTime;

        delta_y = ResolveVerticalCollision(delta_y);

        transform.Translate(new Vector3(delta_x, delta_y, 0f));
    }

    private float ResolveVerticalCollision(float expected_delta_y)
    {

        if (expected_delta_y >= 0) return expected_delta_y;

        Vector2 bottom_center = (Vector2)col.bounds.center + Vector2.down * col.bounds.extents.y;
        Vector2 box_size = new Vector2(col.bounds.size.x * 0.9f, 0.01f);

        RaycastHit2D hit =
            Physics2D.BoxCast(bottom_center, box_size, 0,
            Vector2.down, Mathf.Abs(expected_delta_y), ground_layer);

        if (hit.collider != null)
        {
            velocity_y = 0f;
            return -Mathf.Max(0f, hit.distance - 0.01f);
        }

        return expected_delta_y;
    }
}
