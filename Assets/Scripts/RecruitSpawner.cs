using UnityEngine;

public class RecruitSpawner : MonoBehaviour
{
    [SerializeField] private GameObject recruitPickupPrefab;
    [SerializeField] private float spawnInterval = 2f;
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
            SpawnRecruit();
        }
    }

    private void SpawnRecruit()
    {
        int lane = Random.Range(-1, 2);

        float xPos = lane * laneOffset;

        Vector3 spawnPosition = new Vector3(xPos, spawnY, spawnZ);

        Instantiate(recruitPickupPrefab, spawnPosition, Quaternion.identity);
    }
}