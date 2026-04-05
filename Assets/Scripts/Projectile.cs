using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float destroyZ = 50f;
    [SerializeField] private float damage = 1f;

    private void Update()
    {
        transform.position += Vector3.forward * speed * Time.deltaTime;

        if (transform.position.z >= destroyZ)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy"))
        {
            return;
        }

        EnemyStats enemyStats = other.GetComponentInParent<EnemyStats>();
        if (enemyStats == null)
        {
            enemyStats = other.transform.root.GetComponent<EnemyStats>();
        }

        if (enemyStats != null)
        {
            enemyStats.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}