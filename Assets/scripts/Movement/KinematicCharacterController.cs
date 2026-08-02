using UnityEngine;

public class KinematicCharacterController : MonoBehaviour
{
    public float velocity_x { get; private set; } = 0f;
    public float velocity_y { get; private set; } = 0f;


    [Header("Collision Prediction")]
    [SerializeField] private LayerMask ground_layer;
    [SerializeField] private LayerMask oneway_platform_layer;
    private const float SKIN_WIDTH = 0.02f;
    private const float EPSILON = 0.001f;
    private const float COLLISION_BOX_SCALE = 0.95f;
    private const float BOX_CAST_THICKNESS = 0.01f;
    private const float EXTRA_GROUND_DISTANCE = 0.05f;
    private Collider2D col;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
    }
    public void SetVelocityXY(float x, float y)
    {
        velocity_x = x;
        velocity_y = y;
    }
    public void SetVelocityX(float x)
    {
        velocity_x = x;
    }
    public void SetVelocityY(float y)
    {
        velocity_y = y;
    }
    public void PhysicsUpdate()
    {
        ApplyMovement();
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
