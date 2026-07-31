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

    [Header("Gravity")]
    [SerializeField] private float gravity_scale = 5f;
    [SerializeField] private float fall_gravity_multiplier = 1.5f;

    [Header("Jump")]
    [SerializeField] private float jump_height = 5f;
    [SerializeField] private float jump_force;
    [SerializeField] private float max_fall_speed = -20f;
    [SerializeField] float jump_cut_multiplier = 0.5f;
    private bool jump_held;
    private GroundSensor ground_sensor;

    [Header("Dash")]
    private bool can_dash = true;

    [Header("Collision Prediction")]
    [SerializeField] private LayerMask ground_layer;
    [SerializeField] private LayerMask oneway_platform_layer;
    private Collider2D col;

    void Start()
    {
        ground_sensor = GetComponent<GroundSensor>();
        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        GetPlayerInput();
    }
    void FixedUpdate()
    {
        CaculateVerticalVelocity();

        ApplyMovement();
    }

    private void GetPlayerInput()
    {
        move_direction = 0f;
        if (Input.GetKey(KeyCode.LeftArrow))
            move_direction = -1f;
        if (Input.GetKey(KeyCode.RightArrow))
            move_direction = 1f;

        if (Input.GetKeyDown(KeyCode.Z) && ground_sensor.CheckGrounded())
        {
            velocity_y = Mathf.Sqrt(jump_height * -2 * (Physics2D.gravity.y * gravity_scale));
        }

        if (Input.GetKeyUp(KeyCode.Z) && velocity_y > 0)
        {
            velocity_y *= jump_cut_multiplier;
        }

    }

    private void CaculateVerticalVelocity()
    {
        float gravity = Physics2D.gravity.y * gravity_scale;

        if (velocity_y < 0)
            gravity *= fall_gravity_multiplier;

        velocity_y += gravity * Time.fixedDeltaTime;
        velocity_y = Mathf.Max(velocity_y, max_fall_speed);
    }

    private void ApplyMovement()
    {
        float delta_x = move_direction * move_speed * Time.fixedDeltaTime;
        float delta_y = velocity_y * Time.fixedDeltaTime;

        delta_y = ResolveVerticalCollision(delta_y);
        delta_x = ResolveHorizontalCollision(delta_x);

        transform.Translate(new Vector3(delta_x, delta_y, 0f));
    }

    private float ResolveVerticalCollision(float expected_delta_y)
    {

        if (Mathf.Abs(expected_delta_y) == 0f)
            return expected_delta_y;

        float direction = (expected_delta_y < 0f ? -1f : 1f);
        float skin_width = 0.02f;

        LayerMask colision_mask = ground_layer;
        if (direction < 0)
            colision_mask |= oneway_platform_layer;

        Vector2 bottom_center = (Vector2)col.bounds.center + Vector2.up * direction * (col.bounds.extents.y - skin_width);
        Vector2 box_size = new Vector2(col.bounds.size.x * 0.9f, 0.01f);

        RaycastHit2D hit =
            Physics2D.BoxCast(bottom_center, box_size, 0,
            Vector2.up * direction, Mathf.Abs(expected_delta_y) + skin_width, colision_mask);

        if (hit.collider != null)
        {
            if (direction < 0 && hit.distance == 0 && ((1 << hit.collider.gameObject.layer) & oneway_platform_layer) != 0)
            {
                return expected_delta_y;
            }
            velocity_y = 0f;
            const float epsilon = 0.001f;
            return direction * (hit.distance - skin_width - epsilon);
        }

        return expected_delta_y;
    }

    private float ResolveHorizontalCollision(float expected_delta_x)
    {

        if (Mathf.Abs(expected_delta_x) == 0f)
            return expected_delta_x;

        float direction = (expected_delta_x < 0f ? -1f : 1f);
        float skin_width = 0.02f;

        Vector2 box_size = new Vector2(0.01f, col.bounds.size.y * 0.9f);
        Vector2 size_center = (Vector2)col.bounds.center + Vector2.right * direction * (col.bounds.extents.x - skin_width);
        RaycastHit2D hit = Physics2D.BoxCast(size_center, box_size, 0f,
            Vector2.right * direction, Mathf.Abs(expected_delta_x) + skin_width, ground_layer);

        if (hit.collider != null)
        {
            const float epsilon = 0.001f;
            return direction * (hit.distance - skin_width - epsilon);
        }

        return expected_delta_x;
    }
}
