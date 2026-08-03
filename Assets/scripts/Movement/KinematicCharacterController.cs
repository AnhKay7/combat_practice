using UnityEngine;

public class KinematicCharacterController : MonoBehaviour
{
    public float velocityX { get; private set; } = 0f;
    public float velocityY { get; private set; } = 0f;


    [Header("Collision Prediction")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask onewayPlatformLayer;
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
        velocityX = x;
        velocityY = y;
    }
    public void SetVelocityX(float x)
    {
        velocityX = x;
    }
    public void SetVelocityY(float y)
    {
        velocityY = y;
    }
    public void PhysicsUpdate()
    {
        ApplyMovement();
    }
    private void ApplyMovement()
    {
        float expectedDeltaX = velocityX * Time.fixedDeltaTime;
        float expectedDeltaY = velocityY * Time.fixedDeltaTime;

        float delta_x = ResolveHorizontalCollision(expectedDeltaX);
        float delta_y = ResolveVerticalCollision(expectedDeltaY);

        if (delta_x != expectedDeltaX)
            velocityX = 0f;
        if (delta_y != expectedDeltaY)
            velocityY = 0f;
        transform.Translate(new Vector3(delta_x, delta_y, 0f));
    }

    private float ResolveVerticalCollision(float expectedDeltaY)
    {

        if (Mathf.Abs(expectedDeltaY) == 0f)
            return expectedDeltaY;

        float direction = (expectedDeltaY < 0f ? -1f : 1f);

        LayerMask colisionMask = groundLayer;
        if (direction < 0)
            colisionMask |= onewayPlatformLayer;

        Vector2 bottomCenter = (Vector2)col.bounds.center + Vector2.up * direction * (col.bounds.extents.y - SKIN_WIDTH);
        Vector2 boxSize = new Vector2(col.bounds.size.x * COLLISION_BOX_SCALE, BOX_CAST_THICKNESS);

        RaycastHit2D hit =
            Physics2D.BoxCast(bottomCenter, boxSize, 0,
            Vector2.up * direction, Mathf.Abs(expectedDeltaY) + SKIN_WIDTH, colisionMask);

        if (hit.collider != null)
        {
            bool is_oneway = ((1 << hit.collider.gameObject.layer) & onewayPlatformLayer) != 0;

            if (direction < 0 && is_oneway && hit.distance <= EPSILON)
            {
                return expectedDeltaY;
            }

            return direction * Mathf.Max(hit.distance - SKIN_WIDTH - EPSILON, 0f);
        }

        return expectedDeltaY;
    }

    private float ResolveHorizontalCollision(float expectedDeltaX)
    {

        if (Mathf.Abs(expectedDeltaX) == 0f)
            return expectedDeltaX;

        float direction = (expectedDeltaX < 0f ? -1f : 1f);

        Vector2 boxSize = new Vector2(BOX_CAST_THICKNESS, col.bounds.size.y * COLLISION_BOX_SCALE);
        Vector2 sizeCenter = (Vector2)col.bounds.center + Vector2.right * direction * (col.bounds.extents.x - SKIN_WIDTH);
        RaycastHit2D hit = Physics2D.BoxCast(sizeCenter, boxSize, 0f,
            Vector2.right * direction, Mathf.Abs(expectedDeltaX) + SKIN_WIDTH, groundLayer);

        if (hit.collider != null)
        {
            return direction * Mathf.Max(hit.distance - SKIN_WIDTH - EPSILON, 0f);
        }

        return expectedDeltaX;
    }
}
