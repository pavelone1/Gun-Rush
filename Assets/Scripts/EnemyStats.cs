using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public enum EnemyStatus
    {
        Alive,
        Dead
    }

    [SerializeField] private float maxHealth = 3f;
    [SerializeField] private float currentHealth = 3f;
    [SerializeField] private float contactDamage = 1f;
    [SerializeField] private EnemyStatus status = EnemyStatus.Alive;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public float ContactDamage => contactDamage;
    public EnemyStatus Status => status;

    private void Awake()
    {
        maxHealth = Mathf.Max(1f, maxHealth);
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        status = currentHealth > 0f ? EnemyStatus.Alive : EnemyStatus.Dead;
    }

    public void TakeDamage(float amount)
    {
        if (status == EnemyStatus.Dead || amount <= 0f) return;

        currentHealth -= amount;

        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            Die();
        }
    }

    private void Die()
    {
        status = EnemyStatus.Dead;
        Destroy(gameObject);
    }
}