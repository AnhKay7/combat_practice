using System.Runtime.CompilerServices;
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
    [SerializeField] private float max_jump_buffer = 0.12f;
    private float jump_buffer;
    [SerializeField] private float max_coyote_time = 0.15f;
    private float coyote_time_counter;
    private GroundSensor ground_sensor;

    [Header("Dash")]
    [SerializeField] private float dash_speed = 25f;
    [SerializeField] private bool can_dash = true;
    private bool is_dashing;
    [SerializeField] private float dash_duration = 0.2f;
    private float dash_time_left;
    private float dash_direction;
    [SerializeField] private float dash_cooldown = 0.4f;
    private float dash_cooldown_counter = 0f;
    [SerializeField] private float max_dash_buffer = 0.15f;
    private float dash_buffer;

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
        if (is_dashing)
        {
            ApplyDash();
            return;
        }

        CaculateVerticalVelocity();
        ApplyMovement();
    }

    private void GetPlayerInput()
    {
        HandleMovement();
        HandleJump();
        HandleDash();
    }

    private void HandleJump()
    {
        bool grounded = ground_sensor.CheckGrounded();

        if (Input.GetKeyDown(KeyCode.Z))
            jump_buffer = max_jump_buffer;

        if (jump_buffer > 0f)
            jump_buffer -= Time.deltaTime;

        if (grounded)
            coyote_time_counter = max_coyote_time;
        else if (coyote_time_counter > 0)
            coyote_time_counter -= Time.deltaTime;

        if (jump_buffer > 0f && coyote_time_counter > 0)
        {
            velocity_y = Mathf.Sqrt(jump_height * -2 * (Physics2D.gravity.y * gravity_scale));
            jump_buffer = 0f;
            coyote_time_counter = 0f;

            if (Input.GetKey(KeyCode.Z) == false)
            {
                velocity_y *= jump_cut_multiplier;
            }
        }

        if (Input.GetKeyUp(KeyCode.Z) && velocity_y > 0)
        {
            velocity_y *= jump_cut_multiplier;
        }
    }

    private void HandleDash()
    {
        bool grounded = ground_sensor.CheckGrounded();
        if (grounded) // may touched the ground while in cooldown
            can_dash = true;

        if (Input.GetKeyDown(KeyCode.C))
        {
            dash_buffer = max_dash_buffer;
        }

        if (dash_cooldown_counter > 0)
            dash_cooldown_counter -= Time.deltaTime;

        if (dash_buffer > 0)
        {
            dash_buffer -= Time.deltaTime;
        }

        if (dash_buffer > 0 && can_dash && !is_dashing && dash_cooldown_counter <= 0)
        {
            is_dashing = true;
            can_dash = false;
            dash_time_left = dash_duration;

            dash_direction = move_direction == 0f ? (is_facing_right == true ? 1f : -1f) : move_direction;
        }
    }
    private void ApplyDash()
    {

        dash_time_left -= Time.fixedDeltaTime;

        if (dash_time_left <= 0)
        {
            is_dashing = false;
            velocity_y = 0f;
            dash_cooldown_counter = dash_cooldown;
            return;
        }

        float delta_x = dash_direction * dash_speed * Time.fixedDeltaTime;

        delta_x = ResolveHorizontalCollision(delta_x);

        float delta_y = ResolveVerticalCollision(0f);

        transform.Translate(new Vector3(delta_x, delta_y, 0f));
    }
    private void HandleMovement()
    {
        move_direction = 0f;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            move_direction = -1f;
            is_facing_right = false;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            move_direction = 1f;
            is_facing_right = true;
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
