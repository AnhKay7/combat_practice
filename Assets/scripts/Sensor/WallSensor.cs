using UnityEngine;

public class WallSensor : MonoBehaviour
{
    public bool IsTouchingWall { get; private set; } = false;
    public float WallDirection { get; private set; } = 0f;

    [Header("Collision Layer")]
    [SerializeField] private LayerMask groundLayer;

    [Header("Sensor Setting")]
    private const float SKIN_WIDTH = 0.02f;
    private const float EPSILON = 0.001f;
    private const float COLLISION_BOX_SCALE = 0.95f;

    [SerializeField] private Collider2D targetCollider;
    public void CheckWall()
    {

        Vector2 boxSize = new Vector2(SKIN_WIDTH, targetCollider.bounds.extents.y * COLLISION_BOX_SCALE);

        Vector2 rightCenter = (Vector2)targetCollider.bounds.center + Vector2.right * targetCollider.bounds.extents.x;
        Vector2 leftCenter = (Vector2)targetCollider.bounds.center + Vector2.left * targetCollider.bounds.extents.x;

        RaycastHit2D hitRight = Physics2D.BoxCast(rightCenter, boxSize, 0f, Vector2.right, SKIN_WIDTH, groundLayer);
        RaycastHit2D hitLeft = Physics2D.BoxCast(leftCenter, boxSize, 0f, Vector2.left, SKIN_WIDTH, groundLayer);

        if (hitRight.collider != null)
        {
            IsTouchingWall = true;
            WallDirection = 1f;
        }
        else if (hitLeft.collider != null)
        {
            IsTouchingWall = true;
            WallDirection = -1f;
        }
        else
        {
            IsTouchingWall = false;
            //wall_direction = 0f; save for wall_coyote
        }
    }
}
