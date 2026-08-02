using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float velocity_x { get; private set; } = 0f;
    public float velocity_y { get; private set; } = 0f;

    public bool is_grounded { get; private set; }
}
