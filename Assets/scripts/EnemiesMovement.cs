using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemiesMovement : MonoBehaviour
{
    //Reffrnc
    public Transform Player, Allies;
    public Transform GroundDetector;
    public GameObject Target;
    UnityEngine.AI.NavMeshAgent navm;
    public Rigidbody rb;
    public LayerMask whatIsGround, whatisPlayer, whatisAlly, ThisLayer;

    [SerializeField]
    private bool loudDetected; // Biến để xác định xem âm thanh đã được phát hiện hay không

    public bool LoudDetected
    {
        get { return loudDetected; }
        private set { loudDetected = value; }
    }
    public float LoudSearchingTime, detectTime, reinforceDistance;
    
    //patroling
    public Vector3 walkpoint;
    bool walkPointset;
    public float walkPointRange;

    //Attacking
    [Header("Atk Method")]
    public bool meleeAtk;
    public bool ProjectileAtk;

    public float basicDmg;
    public float TimeBetweenAtk;
    public float AtkRange, SightRange, MvmntSenseRange, SensingTime;
    public float Idlingspd;
    float AdjSpd;
    bool ready2Atk;
    bool alreadyAtkd = true;

    bool PlayerInSenseRange, PlayerInAtkRange, PlayerInSightRange , PlayerInDetectRange, ReinforceLoudCanHeard;

    public Camera cam;
    public Collider PlayerColl;
    private Plane[] planes;

    void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        navm = GetComponent<UnityEngine.AI.NavMeshAgent>();
        ready2Atk = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        PlayerColl = Player.GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        navm.enabled = false;
        Idlingspd = navm.speed;
        AdjSpd = navm.speed * 3f;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!Physics.Raycast(walkpoint, -transform.up, 0.8f, whatIsGround))
        {
            Vector3 grav = Physics.gravity;
            rb.AddForce(grav, ForceMode.Acceleration);
        }
    }
    void Update()
    {
        
        bool isGround = Physics.Raycast(GroundDetector.position, Vector3.down, 0.8f, whatIsGround);
        if (isGround)
        {
            navm.enabled = true;
        }

        planes = GeometryUtility.CalculateFrustumPlanes(cam);
        if (GeometryUtility.TestPlanesAABB(planes, PlayerColl.bounds));
        {
            navm.SetDestination(Player.position);
            Debug.Log("Player sighted");
            Check4Player(); 
            navm.speed = AdjSpd;
            if (ready2Atk)
            {
                Attacking();
            }    
        }
        if(!GeometryUtility.TestPlanesAABB(planes, PlayerColl.bounds))
        {
            if (LoudDetected)
            {
                navm.speed = AdjSpd;
                // Nếu nghe thấy tiếng nổ, đi đến vị trí của nguồn tiếng nổ
                navm.SetDestination(Player.position);
            }
            else
            {
                navm.speed = Idlingspd;
                Patroling();
            }
        }

        PlayerInAtkRange = Physics.CheckSphere(transform.position, AtkRange, whatisPlayer);

        PlayerInSenseRange = Physics.CheckSphere(transform.position, MvmntSenseRange, whatisPlayer);
        PlayerInSightRange = Physics.CheckSphere(transform.position, SightRange, whatisPlayer);
        var targetRender = Player.GetComponent<Renderer>();
        
    }
    void Check4Player()
    { 
        RaycastHit hit;
        Debug.DrawRay(cam.transform.position, transform.forward * 100, Color.green);
        if(Physics.Raycast(cam.transform.position, transform.forward, out hit, 100))
        {
            Chasing();
            Target = hit.collider.gameObject;
        }          
    }

    /*private void IsAlly(Transform a)
    {
        Allies == a.gameObject.tag =="Enemy";
        Allies != this.gameObject;
    }*/
    private void Patroling()
    {
        if (!walkPointset) SearchWalkPoint();

        if (walkPointset)
        {
            navm.SetDestination(walkpoint);           
        }

        Vector3 distance2Walkpoint = transform.position - walkpoint;

        //walkpoint reached
        if(distance2Walkpoint.magnitude <1f)
            walkPointset = false;
    }

    private void SearchWalkPoint()
    {
        if (LoudDetected)
        {
            // Nếu nghe thấy tiếng nổ, đi đến vị trí của nguồn tiếng nổ
            walkpoint = Player.position;
            navm.speed = AdjSpd;
            walkPointset = true;
            navm.enabled = true;
        }
        else
        {
            float randomZ = Random.Range(-walkPointRange, walkPointRange);
            float randomX = Random.Range(-walkPointRange, walkPointRange);

            walkpoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

            if (Physics.Raycast(walkpoint, -transform.up, 1f, whatIsGround))
            {
                walkPointset = true;
                navm.enabled = true;
            }
        }
    }

    public void Chasing()
    {
        navm.SetDestination(Player.position);
    }

    private void Attacking()
    {
        ready2Atk = false;
        navm.SetDestination(Player.position);
        Debug.Log("Attacked");

        if(meleeAtk)
        {
            Collider[] c = Physics.OverlapSphere(transform.position, AtkRange, whatisPlayer);

            foreach (Collider col in c)
            {
                if (col.gameObject.CompareTag("Player"))
                {
                    transform.LookAt(Player);
                    if (col.gameObject.TryGetComponent(out AttributesManager a))
                    {
                        if (a != null)
                            a.TakeDmg(basicDmg);
                    }
                }
            }

        }

        if (alreadyAtkd)
        {
            Invoke("ResetAttack", TimeBetweenAtk);
            alreadyAtkd = false;
        }    

    }

    public void SetLoudDetected(bool value)
    {
        LoudDetected = value;
        if (value)
        {
            transform.LookAt(Player);

            StartCoroutine(ResetLoudDetectedAfterDelay(LoudSearchingTime));
        }
        Debug.Log("LoudDetected set to: " + value);
    }

    private IEnumerator ResetLoudDetectedAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        LoudDetected = false;
        Debug.Log("LoudDetected set to: false after " + delay + " seconds.");
    }

    
    /*private void Reinforcing()
    {
        Collider[] Enemies = Physics.OverlapSphere(transform.position, ReinforceDistance, damageable);
        foreach (Collider enemy in Enemies)
        EnemiesMovement e = enemy.GetComponent<EnemiesMovement>(); 
        if(e != null)
        {
            
        }
    }
    //Check 4 rein4cement from allies
    
    private void FollowingAlly()
    {
        this.gameObject.transform.position = Vector3.Lerp(this.gameObject.transform.position, Allies.transform.position, Time.deltaTime * 0.2f);
        
    }*/

    private void ResetAttack()
    {
        ready2Atk = true;
        alreadyAtkd = true;
    }

    
    /*public void TakeDmg(int dmg)
    {
        health -= dmg;
        if (health < 0)
            Invoke(nameof(DestroyEnemy), 5f);

    }*/

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    void CheckGround()
    {
        
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AtkRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, SightRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, reinforceDistance );

    }
}
