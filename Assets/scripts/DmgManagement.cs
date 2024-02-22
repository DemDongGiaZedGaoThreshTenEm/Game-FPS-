using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DmgManagement : MonoBehaviour
{
    private int i;
    public float dmg;
    [Header("Area Dmg")]
    public bool AreaDmg;
    public float Radius;
    private float ActualDmg;
    private List<float> distances;
    public LayerMask damagable;

    public GameObject ExplosionFX;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        //dmg = wp.LoadOut[0].Damage;
        
    }
    void OnCollisionEnter(Collision c)
    {
        //Direct hit
        if(!AreaDmg)
        {
            if (c.gameObject.TryGetComponent<AttributesManager>(out AttributesManager target))
            {
                target.TakeDmg(dmg);
            }

        }

        //Explode
        if (AreaDmg)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, Radius, damagable);

            foreach (Collider col in colliders)
            {
                // Check if the nearby object has a Damageable component                      
                float distance = Vector3.Distance(transform.position, col.transform.position);
                ActualDmg = (1 - distance / Radius) * dmg;
                AttributesManager nearby = col.gameObject.GetComponentInParent<AttributesManager>();
                if (nearby != null)
                {
                    // Apply area damage to the nearby object
                    if(distance != 0f)
                    {
                        dmg = ActualDmg;
                        nearby.TakeDmg(dmg);
                    }
                    if (distance == 0f)
                    {
                        nearby.TakeDmg(dmg);
                    }    
                }
            }

        }
        Instantiate(ExplosionFX, transform.position, Quaternion.identity);
        ExplosionFX.SetActive(true);
        Destroy(gameObject);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Radius);
    }

}
