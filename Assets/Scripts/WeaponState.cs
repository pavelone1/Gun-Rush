using UnityEngine;

public class WeaponState : MonoBehaviour
{
    [SerializeField] private int weaponLevel = 1;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float fireRate = 0.4f;

    public int WeaponLevel => weaponLevel;
    public GameObject BulletPrefab => bulletPrefab;
    public float FireRate => fireRate;

    public void SetWeaponState(int level, GameObject bullet, float rate)
    {
        weaponLevel = level;
        bulletPrefab = bullet;
        fireRate = rate;
    }

    public void CopyFrom(WeaponState other)
    {
        if (other == null) return;

        weaponLevel = other.weaponLevel;
        bulletPrefab = other.bulletPrefab;
        fireRate = other.fireRate;
    }
}