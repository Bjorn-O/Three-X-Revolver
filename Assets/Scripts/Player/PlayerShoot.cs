using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

public class PlayerShoot : MonoBehaviour
{
    private const int MAX_AMMO = 3;

    public UnityEvent onShoot;
    public UnityEvent onOutOfAmmo;
    public UnityEvent<Vector3> onMouseAimEvent;
    public UnityEvent<Vector2> onStickAimEvent;
    
    private Camera _cam;
    [SerializeField] private float missTolerance;
    [SerializeField] private float laserRicochetLength = 5;
    [SerializeField] private LayerMask laserPointMask;
    [SerializeField] private Transform cursor;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private LineRenderer aimingLine;
    
    private Vector2 _aimPos;
    private Vector2 _aimTarget;

    [SerializeField] private Bullet bulletPrefab;
    private ObjectPool<Bullet> _bulletPool;

    [SerializeField] private float bulletSpeed = 5;
    [SerializeField] private int ammo = 3;
    [SerializeField] private float shootDelayOnResume = 0.1f;
    [SerializeField] private Animator muzzleFlash;
    [SerializeField] private float muzzleSpeedTimeStop = 0.25f;

    private bool canShoot = true;

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
        
        var position = shootPoint.position;
        _aimTarget = _aimPos - (Vector2)position;
        var hit = Physics2D.Raycast(shootPoint.position, -shootPoint.up, 100, laserPointMask);
        aimingLine.SetPosition(0, position);
        
        
        
        if (hit)
        {
            aimingLine.SetPosition(1, hit.point);
            aimingLine.SetPosition(2, hit.point );

            if (!hit.collider.CompareTag("Ricochet")) return;
            //var ricochetDir = Vector2.Reflect(_rb.velocity.normalized, normal);
            var ricochetDir = Vector2.Reflect(-shootPoint.up, hit.normal);
            aimingLine.SetPosition(2, hit.point + ricochetDir * laserRicochetLength);
        }
        else
        {
            aimingLine.SetPosition(1, -shootPoint.up * missTolerance);
            aimingLine.SetPosition(2, -shootPoint.up * missTolerance );
        }
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
        if (ammo <= 0 || TimeManager.instance.TimeScale >= 1 || !canShoot)
            return;

        Bullet bullet = _bulletPool.Get();

        Vector2 currentPos = shootPoint.position;
        

        bullet.Shoot(-shootPoint.up, 
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer != 3)
            return;

        canShoot = false;
        aimingLine.enabled = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer != 3)
            return;

        canShoot = true;
        aimingLine.enabled = true;
    }
}
