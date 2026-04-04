using UnityEngine;

public class RecruitPickup : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float destroyZ = -5f;

    private SquadManager squadManager;

    private void Start()
    {
        squadManager = GameObject.FindWithTag("Player").GetComponent<SquadManager>();
    }
        private void Update()
    {
        transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);

        if (transform.position.z < destroyZ)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        squadManager.AddSquadMember();
        Destroy(gameObject);
    }
}