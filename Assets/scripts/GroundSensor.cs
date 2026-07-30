using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GroundSensor : MonoBehaviour
{

    private Collider2D col;
    [Header("Setting")]
    [SerializeField] private LayerMask ground_layer;
    [SerializeField] private float extra_height = 0.02f;

    public bool is_ground { get; private set; }
    void Start()
    {
        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        //is_ground = CheckGrounded();
    }
    private void FixedUpdate()
    {
        //is_ground = CheckGrounded();
    }
    public bool CheckGrounded()
    {
        Vector2 box_center = col.bounds.center;

        Vector2 box_size = new Vector2(col.bounds.size.x * 0.9f, extra_height);

        float distance = col.bounds.extents.y + extra_height;

        RaycastHit2D hit = Physics2D.BoxCast(box_center, box_size, 0, Vector2.down, distance, ground_layer);

        return hit.collider != null;
    }

    private void OnDrawGizmos()
    {
        if (col == null) col = GetComponent<Collider2D>();
        if (col == null) return;

        Vector2 box_center = (Vector2)col.bounds.center + Vector2.down * (col.bounds.extents.y + extra_height / 2);
        Vector2 box_size = new Vector2(col.bounds.size.x * 0.9f, extra_height);

        Gizmos.color = is_ground ? Color.green : Color.red;

        Gizmos.DrawWireCube(box_center, box_size);
    }
}
