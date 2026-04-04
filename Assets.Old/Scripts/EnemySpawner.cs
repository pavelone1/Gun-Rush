using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnInterval = 1.5f;
    [SerializeField] private float spawnZ = 30f;
    [SerializeField] private float spawnY = 1f;
    [SerializeField] private float laneOffset = 3.2f;
    [SerializeField] private float laneSpread = 1f;

    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        int lane = Random.Range(-1, 2);

        float randomOffset = Random.Range(-laneSpread, laneSpread);
        float xPos = (lane * laneOffset) + randomOffset;

        Vector3 spawnPosition = new Vector3(xPos, spawnY, spawnZ);

        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}