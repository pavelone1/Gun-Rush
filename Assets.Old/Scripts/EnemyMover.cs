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
}