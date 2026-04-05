using UnityEngine;

public class EnemyMover : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float destroyZ = -10f;
 
    private void Update()
    {
        transform.position += Vector3.back * moveSpeed * Time.deltaTime;

        if (transform.position.z <= destroyZ)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            return;
        }

        EnemyStats enemyStats = GetComponent<EnemyStats>();
        float damage = enemyStats != null ? enemyStats.ContactDamage : 1f;

        Health playerHealth = other.GetComponentInParent<Health>();
        if (playerHealth == null)
        {
            playerHealth = other.transform.root.GetComponent<Health>();
        }

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        SquadMemberHealth squadHealth = other.GetComponentInParent<SquadMemberHealth>();
        if (squadHealth == null)
        {
            squadHealth = other.transform.root.GetComponent<SquadMemberHealth>();
        }

        if (squadHealth != null)
        {
            squadHealth.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}