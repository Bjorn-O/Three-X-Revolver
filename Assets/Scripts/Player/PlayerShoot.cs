using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

public class PlayerShoot : MonoBehaviour
{
    private Camera cam;
    [SerializeField] private Transform cursor;
    private Vector2 aimPos;

    [SerializeField] private Bullet bulletPrefab;
    private ObjectPool<Bullet> bulletPool;

    [SerializeField] private float bulletSpeed = 5;
    [SerializeField] private float ammo = 3;

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
        if (ammo <= 0)
            return;

        Bullet bullet = bulletPool.Get();

        Vector2 currentPos = new Vector2(transform.position.x, transform.position.y);

        bullet.Shoot((aimPos - currentPos).normalized, bulletSpeed * TimeManager.instance.TimeScale, currentPos);

        ammo -= 1; 
    }
}
