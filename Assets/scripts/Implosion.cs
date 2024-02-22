using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Implosion : MonoBehaviour
{
    [SerializeField] public float dmg;
    [SerializeField] public float Radius;
    [SerializeField] public float Gravity;
    [SerializeField] public LayerMask damageable;
    bool isImploding = false;
    UnityEngine.AI.NavMeshAgent navm;
    public Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        StartImplosion();

    }
    private void StartImplosion()
    {
        Collider[] Enemies = Physics.OverlapSphere(transform.position, Radius, damageable);
        foreach (Collider enemy in Enemies)
        {
            Transform GroundDetector = enemy.gameObject.GetComponentInParent<EnemiesMovement>().GroundDetector;
            LayerMask ground = enemy.gameObject.GetComponentInParent<EnemiesMovement>().whatIsGround;
            bool isGround = Physics.Raycast(GroundDetector.position, Vector3.down, 0.8f, ground);
            AttributesManager nearby = enemy.gameObject.GetComponentInParent<AttributesManager>();
            navm = enemy.GetComponentInParent<UnityEngine.AI.NavMeshAgent>();
            rb = enemy.gameObject.GetComponentInParent<Rigidbody>();
            if (nearby.gameObject.CompareTag("Enemy"))
            {
                StartCoroutine(ImplosionCoroutine(nearby));
                if (nearby != null)
                {
                    nearby.TakeDmg(dmg);
                    
                    navm.enabled = false;
                    
                    ResetVelocity();
                }
            }
        }
    }

    private IEnumerator ImplosionCoroutine(AttributesManager obj)
    {
        isImploding = true;
        Vector3 initialPosition = obj.gameObject.transform.position;
        rb = obj.gameObject.GetComponentInParent<Rigidbody>();

        float elapsedTime = 0f;
        Mathf.Clamp01(elapsedTime);

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * Gravity;
            rb.AddForce((transform.position - initialPosition).normalized * Gravity, ForceMode.Force);
            yield return null;
        }
    }

    public void ResetVelocity()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
    
}
