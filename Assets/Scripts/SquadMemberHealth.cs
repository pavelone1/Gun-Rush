using System;
using UnityEngine;

public class SquadMemberHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 5f;
    [SerializeField] private float currentHealth = 5f;
    [SerializeField] private bool destroyOnDeath = true;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;

    public event Action<SquadMemberHealth> Died;

    private void Awake()
    {
        maxHealth = Mathf.Max(1f, maxHealth);
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
    }

    public void SetHealth(float newMaxHealth, bool fillToMax)
    {
        maxHealth = Mathf.Max(1f, newMaxHealth);
        currentHealth = fillToMax ? maxHealth : Mathf.Clamp(currentHealth, 0f, maxHealth);
    }

    public void AddMaxHealth(float amount, float currentHealthBonus)
    {
        if (amount <= 0f) return;

        maxHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth + currentHealthBonus, 0f, maxHealth);
    }

    public void TakeDamage(float amount)
    {
        if (amount <= 0f) return;

        currentHealth -= amount;

        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            Die();
        }
    }

    private void Die()
    {
        Died?.Invoke(this);

        if (destroyOnDeath)
        {
            Destroy(gameObject);
        }
    }
}