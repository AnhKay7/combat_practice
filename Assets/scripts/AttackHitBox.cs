using UnityEngine;

public class AttackHitBox : MonoBehaviour
{
    [SerializeField] private int damage = 10;

    private Collider2D hitbox;

    private void Awake()
    {
        hitbox = GetComponent<Collider2D>();
        hitbox.enabled = false;
    }
    public void Activate()
    {
        hitbox.enabled = true;
    }
    public void Deactivate()
    {
        hitbox.enabled = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IDamageable>(out var damageable))
        {
            Debug.Log("Hit!!!");
            damageable.TakeDamage(damage);
        }
    }
}
