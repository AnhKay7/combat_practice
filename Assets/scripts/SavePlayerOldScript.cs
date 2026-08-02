using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class SavePlayerOldScript : MonoBehaviour
{
    [Header("Object")]
    private float velocity_y = 0f;
    private float velocity_x = 0f;

    [Header("Movement")]
    private bool is_facing_right = true;
    [SerializeField] private float max_move_speed = 8f;
    [SerializeField] private float ground_acceleration = 100f;
    [SerializeField] private float ground_deceleration = 120f;
    [SerializeField] private float air_deceleration = 50f;
    [SerializeField] private float turn_acceleration = 380f;

    [Header("Gravity")]
    [SerializeField] private float gravity_scale = 5f;
    [SerializeField] private float fall_gravity_multiplier = 1.5f;

    [Header("Jump")]
    [SerializeField] private float jump_height = 5f;
    [SerializeField] private float max_fall_speed = -20f;
    [SerializeField] float jump_cut_multiplier = 0.5f;
    [SerializeField] private float jump_buffer_time = 0.12f;
    private float jump_buffer;
    [SerializeField] private float coyote_time = 0.15f;
    private float coyote_time_counter;

    [Header("Dash")]
    [SerializeField] private float dash_speed = 25f;
    private bool can_dash;
    [SerializeField] private float dash_duration = 0.2f;
    private float dash_time_left;
    private float dash_direction;
    [SerializeField] private float dash_cooldown = 0.4f;
    private float dash_cooldown_counter = 0f;
    [SerializeField] private float dash_buffer_time = 0.15f;
    private float dash_buffer;

    [Header("Wall Interact")]
    private float wall_direction;
    [SerializeField] private float wall_slide_speed = -5f;
    [SerializeField] private float wall_coyote_time = 0.08f;
    private float wall_coyote_counter;
    [SerializeField] private float wall_jump_force = 12f;
    [SerializeField] private float wall_jump_duration = 0.15f;
    private float wall_jump_timer;
    [SerializeField] private float wall_jump_air_control = 0.05f;

    [Header("Collision Prediction")]
    [SerializeField] private LayerMask ground_layer;
    [SerializeField] private LayerMask oneway_platform_layer;
    private const float SKIN_WIDTH = 0.02f;
    private Collider2D col;
    private const float EPSILON = 0.001f;
    private const float COLLISION_BOX_SCALE = 0.95f;
    private const float BOX_CAST_THICKNESS = 0.01f;
    private const float EXTRA_GROUND_DISTANCE = 0.05f;

    [Header("Input")]
    private bool jump_pressed;
    private bool jump_released;
    private bool jump_held;
    private float move_direction;
    private bool dash_pressed;

    [Header("State")]
    private bool is_grounded;
    private bool is_touching_wall;
    private bool is_dashing;
    void Start()
    {
        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        GetPlayerInput();
        UpdateGroundStatus();
        UpdateWallStatus();
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
        CaculateHorizontalVelocity();
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
            jump_buffer = jump_buffer_time;
        else if (jump_buffer > 0f)
            jump_buffer -= Time.deltaTime;

        if (is_grounded)
            coyote_time_counter = coyote_time;
        else if (coyote_time_counter > 0)
            coyote_time_counter -= Time.deltaTime;

        if (is_grounded || is_touching_wall) // may touched the ground / wall while in cooldown
            can_dash = true;
        if (dash_pressed)
            dash_buffer = dash_buffer_time;
        if (dash_buffer > 0)
            dash_buffer -= Time.deltaTime;
        if (dash_cooldown_counter > 0)
            dash_cooldown_counter -= Time.deltaTime;

        if (is_touching_wall)
            wall_coyote_counter = wall_coyote_time;
        else if (wall_coyote_counter > 0)
            wall_coyote_counter -= Time.deltaTime;

        if (wall_jump_timer > 0)
            wall_jump_timer -= Time.deltaTime;
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
        Vector2 box_size = new Vector2(col.bounds.size.x * COLLISION_BOX_SCALE, BOX_CAST_THICKNESS);
        Vector2 bottom_center = (Vector2)col.bounds.center + Vector2.down * (col.bounds.extents.y - SKIN_WIDTH);

        RaycastHit2D hit = Physics2D.BoxCast(bottom_center, box_size, 0, Vector2.down, SKIN_WIDTH + EXTRA_GROUND_DISTANCE, GetLayerMask());

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
    private void UpdateWallStatus()
    {

        //if (velocity_y == 0f)
        //    is_touching_wall = false;

        Vector2 box_size = new Vector2(SKIN_WIDTH, col.bounds.extents.y * COLLISION_BOX_SCALE);

        Vector2 right_center = (Vector2)col.bounds.center + Vector2.right * col.bounds.extents.x;
        Vector2 left_center = (Vector2)col.bounds.center + Vector2.left * col.bounds.extents.x;

        RaycastHit2D hit_right = Physics2D.BoxCast(right_center, box_size, 0f, Vector2.right, SKIN_WIDTH, ground_layer);
        RaycastHit2D hit_left = Physics2D.BoxCast(left_center, box_size, 0f, Vector2.left, SKIN_WIDTH, ground_layer);

        if (hit_right.collider != null)
        {
            is_touching_wall = true;
            wall_direction = 1f;
        }
        else if (hit_left.collider != null)
        {
            is_touching_wall = true;
            wall_direction = -1f;
        }
        else
        {
            is_touching_wall = false;
            //wall_direction = 0f; save for wall_coyote
        }
    }
    private void ExecuteJump()
    {
        velocity_y = Mathf.Sqrt(jump_height * -2 * (Physics2D.gravity.y * gravity_scale));
        jump_buffer = 0f;
        coyote_time_counter = 0f;
        wall_coyote_counter = 0f;
        if (jump_held == false)
        {
            velocity_y *= jump_cut_multiplier;
        }
    }
    private void HandleJump()
    {
        if (jump_buffer > 0f && !is_dashing)
        {
            if (coyote_time_counter > 0f)
            {
                ExecuteJump();
            }
            else if (wall_coyote_counter > 0f)
            {
                HandleWallJump();
            }
        }

        if (jump_released && velocity_y > 0)
            velocity_y *= jump_cut_multiplier;
    }
    private void HandleWallJump()
    {
        if (jump_buffer > 0 && wall_coyote_counter > 0)
        {
            velocity_x = -wall_direction * wall_jump_force;
            wall_jump_timer = wall_jump_duration;
            ExecuteJump();
        }
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

    private void CaculateHorizontalVelocity()
    {
        float air_control = 1f;
        if (wall_jump_timer > 0)
        {
            air_control = wall_jump_air_control;
        }

        if (move_direction != 0)
        {
            bool is_turning = Mathf.Sign(move_direction) != Mathf.Sign(velocity_x)
                                && Mathf.Abs(velocity_x) > EPSILON;
            float cur_acceleration = is_turning ? turn_acceleration : ground_acceleration;
            if (Mathf.Abs(velocity_x) > max_move_speed && Mathf.Sign(move_direction) == Mathf.Sign(velocity_x))
                velocity_x = Mathf.MoveTowards(velocity_x, Mathf.Sign(velocity_x) * max_move_speed, air_deceleration * Time.fixedDeltaTime);
            else
            {
                velocity_x += move_direction * cur_acceleration * air_control * Time.fixedDeltaTime;
                velocity_x = Mathf.Clamp(velocity_x, -max_move_speed, max_move_speed);
            }
        }
        else
        {
            float cur_deceleration = is_grounded ? ground_deceleration : air_deceleration;
            velocity_x = Mathf.MoveTowards(velocity_x, 0f, cur_deceleration * Time.fixedDeltaTime);
        }
    }
    private void CaculateVerticalVelocity()
    {
        float gravity = Physics2D.gravity.y * gravity_scale;

        if (velocity_y < 0)
            gravity *= fall_gravity_multiplier;

        velocity_y += gravity * Time.fixedDeltaTime;

        if (!is_grounded && is_touching_wall && velocity_y < 0f && wall_direction == move_direction)
        {
            velocity_y = Mathf.Max(wall_slide_speed, velocity_y);
        }
        else
        {
            velocity_y = Mathf.Max(velocity_y, max_fall_speed);
        }
    }

    private void ApplyMovement()
    {
        float expected_delta_x = velocity_x * Time.fixedDeltaTime;
        float expected_delta_y = velocity_y * Time.fixedDeltaTime;

        float delta_x = ResolveHorizontalCollision(expected_delta_x);
        float delta_y = ResolveVerticalCollision(expected_delta_y);

        if (delta_x != expected_delta_x)
            velocity_x = 0f;
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
        Vector2 box_size = new Vector2(col.bounds.size.x * COLLISION_BOX_SCALE, BOX_CAST_THICKNESS);

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

        Vector2 box_size = new Vector2(BOX_CAST_THICKNESS, col.bounds.size.y * COLLISION_BOX_SCALE);
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
