using UnityEngine;

public class GroundSensor : MonoBehaviour
{
    public bool IsGrounded { get; private set; } = false;

    [Header("Collision Layer")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask onewayPlatformLayer;

    [Header("Sensor Setting")]
    private const float SKIN_WIDTH = 0.02f;
    private const float EPSILON = 0.001f;
    private const float COLLISION_BOX_SCALE = 0.95f;
    private const float BOX_CAST_THICKNESS = 0.01f;
    private const float EXTRA_GROUND_DISTANCE = 0.05f;

    [SerializeField] private Collider2D targetCollider;
    private bool IsStandingOnOneWayPlatform(RaycastHit2D hit)
    {
        return hit.distance >= SKIN_WIDTH;
    }
    public void CheckGround(float velocityY)
    {
        if (velocityY > 0f)
        {
            IsGrounded = false;
            return;
        }
        Vector2 boxSize = new Vector2(targetCollider.bounds.size.x * COLLISION_BOX_SCALE, BOX_CAST_THICKNESS);
        Vector2 bottomCenter = (Vector2)targetCollider.bounds.center + Vector2.down * (targetCollider.bounds.extents.y - SKIN_WIDTH);

        LayerMask layerToCheck = groundLayer | onewayPlatformLayer;
        RaycastHit2D hit = Physics2D.BoxCast(bottomCenter, boxSize, 0, Vector2.down, SKIN_WIDTH + EXTRA_GROUND_DISTANCE, layerToCheck);

        if (hit.collider == null)
        {
            IsGrounded = false;
            return;
        }

        bool isOneway = ((1 << hit.collider.gameObject.layer) & onewayPlatformLayer) != 0;
        if (isOneway)
        {
            IsGrounded = IsStandingOnOneWayPlatform(hit);
        }
        else
        {
            IsGrounded = true;
        }
    }
}
