using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 10f;
    [SerializeField] private float currentHealth = 10f;

    [Header("Death Behavior")]
    [SerializeField] private bool destroyOnDeath = true;
    [SerializeField] private bool triggerGameOverOnDeath = false;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public bool IsDead => currentHealth <= 0f;

    public event Action<Health> Died;

    private void Awake()
    {
        maxHealth = Mathf.Max(1f, maxHealth);
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
    }

    public void SetHealth(float newMaxHealth, bool fillToMax)
    {
        maxHealth = Mathf.Max(1f, newMaxHealth);

        if (fillToMax)
        {
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        }
    }

    public void AddMaxHealth(float amount, float currentHealthBonus)
    {
        if (amount <= 0f) return;

        maxHealth = Mathf.Max(1f, maxHealth + amount);
        currentHealth = Mathf.Clamp(currentHealth + currentHealthBonus, 0f, maxHealth);
    }

    public void TakeDamage(float amount)
    {
        if (IsDead || amount <= 0f) return;

        currentHealth = Mathf.Max(0f, currentHealth - amount);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (IsDead || amount <= 0f) return;

        currentHealth = Mathf.Clamp(currentHealth + amount, 0f, maxHealth);
    }

    private void Die()
    {
        Died?.Invoke(this);

        if (triggerGameOverOnDeath)
        {
            Debug.Log("Game Over");
            Time.timeScale = 0f;
        }

        if (destroyOnDeath)
        {
            Destroy(gameObject);
        }
    }
}