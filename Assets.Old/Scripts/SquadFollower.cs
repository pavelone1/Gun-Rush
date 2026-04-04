using UnityEngine;

public class SquadFollower : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float followSpeed = 10f;

    public void Initialize(Transform playerTarget, Vector3 followOffset)
    {
        player = playerTarget;
        offset = followOffset;
    }

    private void Update()
    {
        if (player == null) return;

        Vector3 targetPosition = player.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }
}