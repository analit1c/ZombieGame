using UnityEngine;
using System.Collections;

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Settings")]
    public Camera playerCamera;
    public Transform muzzle;

    [Header("Shooting")]
    public float damage = 25f;
    public float fireRate = 0.3f;
    public float range = 100f;

    [Header("Ammunition")]
    public int magazineSize = 12;
    public int reserveAmmo = 60;
    public float reloadTime = 1.5f;

    private int currentAmmo;
    private float nextFireTime;
    private bool isReloading;

    void Start()
    {
        currentAmmo = magazineSize;

        if (playerCamera == null)
            playerCamera = Camera.main;

        if (playerCamera == null)
            Debug.LogError("Player Camera не назначена!");
    }

    void Update()
    {
        if (isReloading)
            return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (Time.time < nextFireTime)
            return;

        if (currentAmmo <= 0)
        {
            Debug.Log("Нет патронов! Нажми R.");
            return;
        }

        nextFireTime = Time.time + fireRate;
        currentAmmo--;

        Ray ray = playerCamera.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f)
        );

        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            Debug.Log("Попадание: " + hit.collider.name);

            EnemyAI enemy = hit.collider.GetComponentInParent<EnemyAI>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }

        Debug.Log(
            "Выстрел! Патронов: " +
            currentAmmo + "/" + reserveAmmo
        );

        if (muzzle != null)
        {
            Debug.DrawRay(
                muzzle.position,
                ray.direction * range,
                Color.yellow,
                0.2f
            );
        }
    }

    IEnumerator Reload()
    {
        if (currentAmmo == magazineSize || reserveAmmo <= 0)
            yield break;

        isReloading = true;

        Debug.Log("Перезарядка...");

        yield return new WaitForSeconds(reloadTime);

        int neededAmmo = magazineSize - currentAmmo;
        int ammoToLoad = Mathf.Min(neededAmmo, reserveAmmo);

        currentAmmo += ammoToLoad;
        reserveAmmo -= ammoToLoad;

        isReloading = false;

        Debug.Log(
            "Перезаряжено! Патронов: " +
            currentAmmo + "/" + reserveAmmo
        );
    }
}
