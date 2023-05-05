using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Pool;
using UnityEngine.Serialization;

public class PlayerShoot : MonoBehaviour
{
    private const int MAX_AMMO = 3;

    public UnityEvent onShoot;
    public UnityEvent onOutOfAmmo;
    public UnityEvent<Vector3> onMouseAimEvent;
    public UnityEvent<Vector2> onStickAimEvent;
    
    private Camera _cam;
    [SerializeField] private Transform cursor;
    [SerializeField] private Transform shootPoint;
    private Vector2 _aimPos;

    [SerializeField] private Bullet bulletPrefab;
    private ObjectPool<Bullet> _bulletPool;

    [SerializeField] private float bulletSpeed = 5;
    [SerializeField] private int ammo = 3;
    [SerializeField] private float shootDelayOnResume = 0.1f;
    [SerializeField] private Animator muzzleFlash;
    [SerializeField] private float muzzleSpeedTimeStop = 0.25f;

    // Start is called before the first frame update
    private void Start()
    {
        muzzleFlash.gameObject.SetActive(false);
        _bulletPool = new ObjectPool<Bullet>(() => {
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
        _cam = Camera.main;


        TimeManager.instance.OnTimeStop += () => muzzleFlash.speed = muzzleSpeedTimeStop;
        TimeManager.instance.OnTimeResume += () => muzzleFlash.speed = 1;
    }

    private void Update()
    {
        onMouseAimEvent?.Invoke(_aimPos);
    }

    private void OnMouseAim(InputValue inputValue)
    {
        var mousePos = inputValue.Get<Vector2>();

        Vector2 worldPos = _cam.ScreenToWorldPoint(mousePos);

        cursor.position = worldPos;
        _aimPos = worldPos;
    }

    private void OnStickAim(InputValue inputValue)
    {
        var stickPos = inputValue.Get<Vector2>();
        
        //Aimpost adjustment
    }

    private void OnShoot()
    {
        if (ammo <= 0 || TimeManager.instance.TimeScale >= 1)
            return;

        Bullet bullet = _bulletPool.Get();

        Vector2 currentPos = shootPoint.position;
        

        bullet.Shoot((_aimPos - currentPos).normalized, 
            bulletSpeed * TimeManager.instance.TimeScale, 
            currentPos, 
            (MAX_AMMO - ammo) * shootDelayOnResume);

        bullet.OnRelease = ReleaseBullet;
        SoundManager.instance.PlaySoundEffect("Player", "Shoot");
        muzzleFlash.gameObject.SetActive(false);
        muzzleFlash.gameObject.SetActive(true);
        
        onShoot?.Invoke();
        
        ammo -= 1;

        if (ammo <= 0)
        {
            onOutOfAmmo?.Invoke();
        }
    }

    private void ReleaseBullet(Bullet bullet)
    {
        _bulletPool.Release(bullet);
    }

    private void OnDebugResumeTime()
    {
        TimeManager.instance.ResumeTime();
    }
}
