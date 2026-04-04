using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [SerializeField] private Transform firePoint;

    private WeaponState weaponState;
    private float timer;

    private void Awake()
    {
        weaponState = GetComponent<WeaponState>();
    }

    private void Update()
    {
        if (weaponState == null || weaponState.BulletPrefab == null || firePoint == null)
            return;

        timer += Time.deltaTime;

        if (timer >= weaponState.FireRate)
        {
            timer = 0f;
            Fire();
        }
    }

    private void Fire()
    {
        Instantiate(weaponState.BulletPrefab, firePoint.position, Quaternion.identity);
    }
}