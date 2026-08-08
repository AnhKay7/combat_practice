using System;
using UnityEngine;

public class TrainningDummy : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    private void Awake()
    {
        currentHealth = maxHealth;
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        Debug.Log($"Dummy took {damage}. HP: {currentHealth}");

        if (currentHealth <= 0)
            Die();
    }
    private void Die()
    {
        Destroy(gameObject);
    }
}
