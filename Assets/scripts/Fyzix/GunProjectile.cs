using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using TMPro;

public class GunProjectile : MonoBehaviour
{
    //Animators
    public enum WeaponState
    {
        Idle,
        Shooting,
        Handling
    }
    private WeaponState currentState = WeaponState.Idle;
    private bool isShooting = false;
    public bool isHandlingAnimationPlaying = false;
    private Animator anim;
    public bool HandlingSupport;
    //bulet
    public GameObject bullet;
    public Transform cam;
    //bullet force
    public float shootForce, upwardForce;

    //Gun stats
    public float timeBetweenShooting, spread, reloadTime, timeBetweenShots, flashtime;
    public int magazineSize, bulletsPerTap, pelletsNum;
    public int maxAmmo;
    public bool allowButtonHold;
    public float speed = 5f;
    private float Loud;
    public float boltingDelay;

    int bulletsleft, bulletsShot;

    //bools
    public bool shooting, readyToShoot, reloading;

    //Reference
    public Transform attackPoint;
    public Transform flashPoint;
    public LayerMask Enemy;
    private EnemiesMovement EM;
    public Guns gun;

    //Graphic
    public GameObject muzzleflash;
    public TextMeshProUGUI ammunitionDisplay;
    //bug fixing
    public bool allowInvoke = true;

    //Recoil
    Vector3 initialGunPos;
    Vector3 initialGunRot;

    [Header("Recoil setting")]
    public float rotationSpeed = 4f;
    public float returnSpeed = 25f;
    public float backSpeed = 25f;


    [Header("Hip fire")]
    public Vector3 RecoilRotation = new Vector3(3f, 3f, 3f);
    public float kickBack = 5f;
    public float snappiness = 5f;


    [Header("ADS")]
    public Vector3 RecoilRotationADS = new Vector3(1.4f, 1.4f, 1.4f);
    public float kickBackADS = 0f;
    [Header("Status")]
    public bool aiming;

    private Vector3 currentRotation;
    private Vector3 currentPos;
    private Vector3 targetPos;

    private Vector3 Rot;

    public Rotation SetY;
    // Start is called before the first frame update\
    void Start()
    {
        anim = this.gameObject.GetComponent<Animator>();
        anim.updateMode = AnimatorUpdateMode.AnimatePhysics;
    }
    private void Awake()
    {
        // Ensure the magazine is full
        bulletsleft = magazineSize;
        readyToShoot = true;

        // Nếu BulletPool.Instance chưa được khởi tạo, khởi tạo nó ở đây
        if (BulletPool.Instance == null)
        {
            GameObject bulletPoolObject = new GameObject("BulletPool");
            BulletPool.Instance = bulletPoolObject.AddComponent<BulletPool>();
            BulletPool.Instance.bulletPrefab = bullet;
            BulletPool.Instance.poolSize = 20; // Hoặc số lượng pool mà bạn mong muốn
        }
    }
    //Fixed Updt (uses 4 recoil)
    void FixedUpdate() 
    {
        currentRotation = Vector3.Slerp(currentRotation, Vector3.zero, returnSpeed * Time.deltaTime);
    }

    // Update is called once per frame

    private void Update()
    {
        //Update Recoil
        cam = Camera.main.transform;
        Rot = Vector3.Slerp(Rot, currentRotation, rotationSpeed * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(Rot);
        initialGunPos = transform.localPosition;
        cam.localRotation = Quaternion.Euler(-Rot*5f);
        
        back();
        /**/
        if (Input.GetMouseButton(1))
        {
            aiming = true;
        }
        else { aiming = false; }
        MyInput();
        //Set ammunition display if it exists
        if (ammunitionDisplay != null)
            ammunitionDisplay.SetText(bulletsleft / bulletsPerTap + "/" + magazineSize / bulletsPerTap);
        if(allowButtonHold && !reloading)
        {
            anim.updateMode = AnimatorUpdateMode.AnimatePhysics;
        }
        
            if (currentState == WeaponState.Handling)
            {
                // If animation "Handling" has finished playing
                if (!isHandlingAnimationPlaying && currentState == WeaponState.Handling)
                {
                    // Transition back to Idle state
                    currentState = WeaponState.Idle;
                    // Switch back to AnimatePhysics mode
                    anim.updateMode = AnimatorUpdateMode.AnimatePhysics;
                    // Handling animation has finished, perform any cleanup
                    HandlingAnimationFinished();
                }
                else
                {
                    // If animation "Handling" is still playing, switch to Normal mode
                    anim.updateMode = AnimatorUpdateMode.Normal;
                }
            }
    }
    private void MyInput()
    {
        if (allowButtonHold) shooting = Input.GetMouseButton(0);

        else shooting = Input.GetKeyDown(KeyCode.Mouse0);
        //Reloading 
        if (Input.GetKeyDown(KeyCode.R) && bulletsleft < magazineSize && !reloading && maxAmmo > 0 && allowInvoke) Reload();

        if (readyToShoot && shooting && !reloading && bulletsleft <= 0 && maxAmmo >0) Reload();
            //Shooting
            if (readyToShoot && shooting && !reloading && bulletsleft >0 && maxAmmo >0)
        {
            ReleaseLoud();
            bulletsShot = 0;
            Shoot();
            fire();
            back();
        }    
    }
    public void Shoot()
    {
        if (reloading || !readyToShoot || bulletsleft <= 0) return;
        isShooting = true;
        readyToShoot = false;
        anim.Play("Shooting");

        // Add forces to bullet
        for (int i = 0; i < pelletsNum; i++)
        {
            // Shotgun
            float x = Random.Range(-spread, spread);
            float y = Random.Range(-spread, spread);

            Vector3 direction = Quaternion.Euler(x, y, Random.Range(-spread, spread)) * attackPoint.forward;

            // Lấy đạn từ pool thay vì tạo mới
            GameObject Bullet = BulletPool.Instance.GetBullet();
            if (Bullet == null)
            {
                // Nếu pool không còn đạn, tạo mới đạn
                Bullet = Instantiate(bullet, attackPoint.position, attackPoint.rotation * Quaternion.identity);
            }
            else
            {
                Bullet.transform.position = attackPoint.position;
                Bullet.transform.rotation = attackPoint.rotation * Quaternion.identity;
            }

            Rigidbody rig = Bullet.GetComponent<Rigidbody>();
            rig.velocity = Vector3.zero; // Đảm bảo vận tốc ban đầu là 0
            rig.angularVelocity = Vector3.zero; // Đảm bảo không có vận tốc góc
            rig.AddForce(attackPoint.forward * speed, ForceMode.Impulse);
            rig.AddForce(direction.normalized * shootForce, ForceMode.Impulse);

            // Trả lại đạn vào pool sau một thời gian nhất định (tùy thuộc vào game của bạn)
            StartCoroutine(ReturnBulletToPool(Bullet, 4.0f)); // Giả sử 2 giây là đủ để đạn tồn tại trên màn hình
        }

        // Instantiate muzzle flash if has one
        if (muzzleflash != null)
        {
            Instantiate(muzzleflash, flashPoint.position, Quaternion.identity);
        }

        bulletsleft--;
        bulletsShot++;
        maxAmmo--;

        // Instantiate loud after one shot
        Loud = gun.MuzzleBlastLoud;
        Collider[] enemyColliders = GetEnemyCollidersInRadius(attackPoint.transform.position, Loud);
        foreach (Collider enemyCollider in enemyColliders)
        {
            EM = enemyCollider.GetComponent<EnemiesMovement>();
            EM.Chasing();
        }

        // Sau khi phát animation, đặt thời gian đợi để đặt lại hasPlayedAnimation thành false
        Invoke("ResetAnimationFlag", timeBetweenShooting);

        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
            anim.updateMode = AnimatorUpdateMode.AnimatePhysics;
            allowInvoke = false;
        }

        if (bulletsShot < bulletsPerTap && bulletsleft > 0)
        {
            Invoke("Shoot", timeBetweenShots);
        }

        if (HandlingSupport)
        {
            if (currentState != WeaponState.Idle)
            {
                return;
            }
            currentState = WeaponState.Shooting;
            Invoke("TransitionToHandlingState", timeBetweenShooting);
            Invoke("SwitchToNormalMode", boltingDelay);
        }
    }
    private IEnumerator ReturnBulletToPool(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        BulletPool.Instance.ReturnBullet(bullet);
    }
    private void TransitionToHandlingState()
    {
        if(HandlingSupport)
        {
            currentState = WeaponState.Handling;
            Debug.Log("Handling");
            anim.Play("Handling");
            isHandlingAnimationPlaying = false;
        }
    }

    public void HandlingAnimationFinished()
    {
        isHandlingAnimationPlaying = false;
        // Kiểm tra và đảm bảo chế độ animation đã được chuyển sang "Normal"
    }

    private void SwitchToNormalMode()
    {
        anim.updateMode = AnimatorUpdateMode.Normal;
        isHandlingAnimationPlaying = true;
    }
    /*IEnumerator Bolting()
    {
        yield return new WaitForSeconds(boltingDelay);
        anim.updateMode = AnimatorUpdateMode.Normal;
        anim.Play("Handling");
        Invoke("ResetAnimationFlag", timeBetweenShooting);
    }*/

    private void ResetAnimationFlag()
    {
        anim.Play("New State");
        isShooting = false;
    }

    Collider[] GetEnemyCollidersInRadius(Vector3 position, float rad)
    {       
        // Use OverlapSphere to find colliders within the specified radius
        Collider[] allColliders = Physics.OverlapSphere(position, rad);

        // Filter colliders to get only those with the "Enemy" tag
        List<Collider> enemyColliders = new List<Collider>();
        foreach (Collider collider in allColliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                enemyColliders.Add(collider);
            }
        }

        return enemyColliders.ToArray();
    }

    

    private void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }

    private void Reload()
    {
        if (reloading || bulletsleft == magazineSize) return;

        anim.Play("Reloading");
        Debug.Log("Reloading");

        reloading = true;
        Invoke("ReloadFinished", reloadTime);
        anim.updateMode = AnimatorUpdateMode.Normal;
    }

    private void ReloadFinished()
    {
        anim.Play("New State");
        anim.updateMode = AnimatorUpdateMode.AnimatePhysics;

        int bulletsToReload = magazineSize - bulletsleft;
        if (maxAmmo < bulletsToReload)
        {
            bulletsToReload = maxAmmo;
        }

        bulletsleft += bulletsToReload;
        maxAmmo -= bulletsToReload;

        reloading = false;
        readyToShoot = true;
    }

    public void ReleaseLoud()
    {
        float Loudness = gun.MuzzleBlastLoud;
        Collider[] AttractedEnemies = Physics.OverlapSphere(attackPoint.localPosition, Loudness, Enemy);
        foreach (Collider enemy in AttractedEnemies)
        {
            Transform parentTransform = enemy.transform.parent;
            if (parentTransform != null)
            {
                GameObject parentObject = parentTransform.gameObject;
                if (parentObject.CompareTag("Enemy"))
                {
                    EnemiesMovement e = parentObject.GetComponent<EnemiesMovement>();
                    if(e != null)
                    {
                        e.SetLoudDetected(true);
                        Debug.Log("Attracted Enemies");
                    }
                }
            }
        }
    }
    public void fire()
    {

        currentPos = Vector3.Lerp(currentPos, targetPos, Time.deltaTime * snappiness);
        transform.localPosition = currentPos;
        if (aiming)
        {
            currentRotation += new Vector3(RecoilRotationADS.x, Random.Range(-RecoilRotationADS.y, RecoilRotationADS.y), Random.Range(-RecoilRotationADS.z, RecoilRotationADS.z));
            currentPos += new Vector3(0, 0, kickBackADS);
        }
        else
        {
            currentRotation += new Vector3(RecoilRotation.x, Random.Range(-RecoilRotation.y, RecoilRotation.y), Random.Range(-RecoilRotation.z, RecoilRotation.z));
            currentPos += new Vector3(0, 0, kickBack);

        }
    }
   
    public void back()
    {
        targetPos = Vector3.Lerp(targetPos, initialGunPos, Time.deltaTime * backSpeed);
        currentPos = Vector3.Lerp(currentPos, targetPos, Time.deltaTime * snappiness);
        transform.localPosition = currentPos;
        cam.localPosition = -currentPos * 0.2f
            ;
    }
    
    public void OnTriggerEnter(Collider bullet)
    {
        Destroy(this.bullet);
    }

    
}
