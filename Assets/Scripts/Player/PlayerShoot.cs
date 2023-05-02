using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

public class PlayerShoot : MonoBehaviour
{
    private static int MAX_AMMO = 3;

    private Camera cam;
    [SerializeField] private Transform cursor;
    [SerializeField] private Transform shootPoint;
    private Vector2 aimPos;

    [SerializeField] private Bullet bulletPrefab;
    private ObjectPool<Bullet> bulletPool;

    [SerializeField] private float bulletSpeed = 5;
    [SerializeField] private int ammo = 3;
    [SerializeField] private float shootDelayOnResume = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        bulletPool = new ObjectPool<Bullet>(() => {
            var bullet = Instantiate(bulletPrefab);
            bullet.Setup();
            return bullet;
        }, bullet => {
            bullet.gameObject.SetActive(true);
        }, bullet => {
             bullet.gameObject.SetActive(false);
        }, bullet => {
            Destroy(bullet.gameObject);
        }, false, 3, 3);

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        cam = Camera.main;
    }

    private void OnAim(InputValue inputValue)
    {
        Vector2 mousePos = inputValue.Get<Vector2>();

        Vector2 worldPos = cam.ScreenToWorldPoint(mousePos);

        cursor.position = worldPos;
        aimPos = worldPos;
    }

    private void OnShoot()
    {
        if (ammo <= 0 || TimeManager.instance.TimeScale >= 1)
            return;

        Bullet bullet = bulletPool.Get();

        Vector2 currentPos = new Vector2(shootPoint.position.x, shootPoint.position.y);

        bullet.Shoot((aimPos - currentPos).normalized, 
            bulletSpeed * TimeManager.instance.TimeScale, 
            currentPos, 
            (MAX_AMMO - ammo) * shootDelayOnResume);

        bullet.OnRelease = ReleaseBullet;

        ammo -= 1;

        if (ammo <= 0)
        {
            TimeManager.instance.ResumeTime();
        }
    }

    private void ReleaseBullet(Bullet bullet)
    {
        bulletPool.Release(bullet);
    }

    private void OnDebugResumeTime()
    {
        TimeManager.instance.ResumeTime();
    }
}
