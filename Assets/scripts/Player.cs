using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Object")]
    [SerializeField] private float velocity_y = 0f;

    [Header("Movement")]
    private bool is_facing_right = true;
    [SerializeField] private float move_speed = 8f;

    [Header("Gravity")]
    [SerializeField] private float gravity_scale = 5f;
    [SerializeField] private float fall_gravity_multiplier = 1.5f;

    [Header("Jump")]
    [SerializeField] private float jump_height = 5f;
    [SerializeField] private float max_fall_speed = -20f;
    [SerializeField] float jump_cut_multiplier = 0.5f;
    [SerializeField] private float max_jump_buffer = 0.12f;
    private float jump_buffer;
    [SerializeField] private float max_coyote_time = 0.15f;
    private float coyote_time_counter;

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
    private bool is_grounded;
    [SerializeField] private const float SKIN_WIDTH = 0.02f;
    private Collider2D col;
    [SerializeField] const float EPSILON = 0.001f;

    [Header("Input")]
    private bool jump_pressed;
    private bool jump_released;
    private bool jump_held;
    private float move_direction;
    private bool dash_pressed;

    void Start()
    {
        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        GetPlayerInput();
        UpdateGroundStatus();
        UpdateTimer();
        HandleJump();
        HandleDash();
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
        if (Input.GetKey(KeyCode.RightArrow))
        {
            move_direction = 1f;
            is_facing_right = true;
        }
    }

    private void UpdateTimer()
    {
        if (jump_pressed)
            jump_buffer = max_jump_buffer;

        if (jump_buffer > 0f)
            jump_buffer -= Time.deltaTime;

        if (is_grounded)
            coyote_time_counter = max_coyote_time;
        else if (coyote_time_counter > 0)
            coyote_time_counter -= Time.deltaTime;

        if (is_grounded) // may touched the ground while in cooldown
            can_dash = true;
        if (dash_pressed)
            dash_buffer = max_dash_buffer;

        if (dash_buffer > 0)
            dash_buffer -= Time.deltaTime;

        if (dash_cooldown_counter > 0)
            dash_cooldown_counter -= Time.deltaTime;
    }
    private LayerMask GetLayerMask()
    {
        LayerMask layer_mask = ground_layer;

        if (velocity_y <= 0f)
            layer_mask |= oneway_platform_layer;

        return layer_mask;
    }
    private void UpdateGroundStatus()
    {
        if (velocity_y > 0f)
        {
            is_grounded = false;
            return;
        }
        Vector2 box_size = new Vector2(col.bounds.size.x * 0.9f, 0.01f);
        Vector2 bottom_center = (Vector2)col.bounds.center + Vector2.down * (col.bounds.extents.y - SKIN_WIDTH);

        RaycastHit2D hit = Physics2D.BoxCast(bottom_center, box_size, 0, Vector2.down, SKIN_WIDTH + 0.05f, GetLayerMask());

        if (hit.collider == null)
        {
            is_grounded = false;
            return;
        }

        bool is_oneway = ((1 << hit.collider.gameObject.layer) & oneway_platform_layer) != 0;
        if (is_oneway)
        {
            is_grounded = hit.distance >= SKIN_WIDTH;
        }
        else
        {
            is_grounded = true;
        }
    }
    private void HandleJump()
    {
        if (jump_buffer > 0f && coyote_time_counter > 0 && !is_dashing)
        {
            velocity_y = Mathf.Sqrt(jump_height * -2 * (Physics2D.gravity.y * gravity_scale));
            jump_buffer = 0f;
            coyote_time_counter = 0f;

            if (jump_held == false)
            {
                velocity_y *= jump_cut_multiplier;
            }
        }

        if (jump_released && velocity_y > 0)
            velocity_y *= jump_cut_multiplier;
    }
    private void HandleDash()
    {

        if (dash_buffer > 0 && can_dash && !is_dashing && dash_cooldown_counter <= 0)
        {
            is_dashing = true;
            can_dash = false;
            dash_time_left = dash_duration;
            dash_buffer = 0f;
            dash_direction = move_direction == 0f ? (is_facing_right == true ? 1f : -1f) : move_direction;
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
        float expected_delta_x = move_direction * move_speed * Time.fixedDeltaTime;
        float expected_delta_y = velocity_y * Time.fixedDeltaTime;

        float delta_y = ResolveVerticalCollision(expected_delta_y);
        float delta_x = ResolveHorizontalCollision(expected_delta_x);

        if (delta_y != expected_delta_y)
            velocity_y = 0f;
        transform.Translate(new Vector3(delta_x, delta_y, 0f));
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

    private float ResolveVerticalCollision(float expected_delta_y)
    {

        if (Mathf.Abs(expected_delta_y) == 0f)
            return expected_delta_y;

        float direction = (expected_delta_y < 0f ? -1f : 1f);

        LayerMask colision_mask = ground_layer;
        if (direction < 0)
            colision_mask |= oneway_platform_layer;

        Vector2 bottom_center = (Vector2)col.bounds.center + Vector2.up * direction * (col.bounds.extents.y - SKIN_WIDTH);
        Vector2 box_size = new Vector2(col.bounds.size.x * 0.9f, 0.01f);

        RaycastHit2D hit =
            Physics2D.BoxCast(bottom_center, box_size, 0,
            Vector2.up * direction, Mathf.Abs(expected_delta_y) + SKIN_WIDTH, colision_mask);

        if (hit.collider != null)
        {
            bool is_oneway = ((1 << hit.collider.gameObject.layer) & oneway_platform_layer) != 0;

            if (direction < 0 && is_oneway && hit.distance <= EPSILON)
            {
                return expected_delta_y;
            }

            return direction * Mathf.Max(hit.distance - SKIN_WIDTH - EPSILON, 0f);
        }

        return expected_delta_y;
    }

    private float ResolveHorizontalCollision(float expected_delta_x)
    {

        if (Mathf.Abs(expected_delta_x) == 0f)
            return expected_delta_x;

        float direction = (expected_delta_x < 0f ? -1f : 1f);

        Vector2 box_size = new Vector2(0.01f, col.bounds.size.y * 0.9f);
        Vector2 size_center = (Vector2)col.bounds.center + Vector2.right * direction * (col.bounds.extents.x - SKIN_WIDTH);
        RaycastHit2D hit = Physics2D.BoxCast(size_center, box_size, 0f,
            Vector2.right * direction, Mathf.Abs(expected_delta_x) + SKIN_WIDTH, ground_layer);

        if (hit.collider != null)
        {
            return direction * Mathf.Max(hit.distance - SKIN_WIDTH - EPSILON, 0f);
        }

        return expected_delta_x;
    }
}
