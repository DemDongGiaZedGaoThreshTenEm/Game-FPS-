using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DmgManagement : MonoBehaviour
{
    public float dmg;
    [Header("Area Dmg")]
    public bool AreaDmg;
    public float Radius;
    public float PenetratingPower;
    public float ImpactLoud;

    private float ActualDmg;
    private float PenetratingDmg;
    private float PenetratingRatio;

    public CamShaking Shake;
    public LayerMask damagable;
    public ProceduralPenetration P;
    public GameObject ExplosionFX;
    public GameObject ExplosionLight;
    public bool ExFxShow;
    private bool exploded; // Backing field
    public bool Exploded
    {
        get { return exploded; }
        private set { exploded = value; }
    }
    private Vector3 previousPosition;

    void Start()
    {
        previousPosition = transform.position;
        if (Shake == null)
        {
            Shake = Camera.main.GetComponent<CamShaking>();
        }
    }

    void Update()
    {
        if (!AreaDmg && ExplosionFX != null)
        {
            ExplosionFX.SetActive(false);
        }

        if (AreaDmg && P != null)
        {
            P.gameObject.SetActive(false);
        }
        // Sub-stepping for collision detection (In case Bullet goes too fast)
        SubStepMovement();
    }

    // Sub-stepping method
    void SubStepMovement()
    {
        int subSteps = 5; // Number of sub-steps
        float stepSize = Vector3.Distance(previousPosition, transform.position) / subSteps;

        for (int i = 0; i < subSteps; i++)
        {
            Vector3 nextPosition = Vector3.MoveTowards(previousPosition, transform.position, stepSize);
            RaycastHit hit;

            if (Physics.Linecast(previousPosition, nextPosition, out hit, damagable))
            {
                OnHit(hit.collider);
                break;
            }

            previousPosition = nextPosition;
        }

        previousPosition = transform.position; // Update for the next frame
    }

    // Called when a hit is detected
    void OnHit(Collider c)
    {
        if (c.gameObject != null)
        {
            Transform parentTransform = c.transform.parent != null ? c.transform.parent : c.transform;
            if (parentTransform != null)
            {
                if (parentTransform.gameObject.TryGetComponent<AttributesManager>(out AttributesManager target))
                {
                    if (P != null)
                    {
                        float depth = P.CalculateDepth(parentTransform.gameObject);
                        PenetratingDmg = (depth / PenetratingPower) * dmg;
                        target.TakeDmg(PenetratingDmg);
                        Debug.Log("Impacted with damage: " + PenetratingDmg);
                    }
                    else
                    {
                        Debug.LogError("P is null. Cannot calculate penetrating damage.");
                    }
                }
                else
                {
                    Debug.LogWarning("AttributesManager not found on " + parentTransform.gameObject.name);
                }
            }
            else
            {
                Debug.LogWarning("Parent transform is null for " + c.gameObject.name);
            }
            if (ExplosionFX != null && !ExFxShow)
            {
                ExplosionFX.SetActive(false);
            }

            // Optionally, destroy the bullet or handle penetration
        }
    }
    //Handle Penetration
    void OnTriggerEnter(Collider c)
    {
        Debug.Log("OnTriggerEnter called");

        if (c.gameObject != null)
        {
            OnHit(c);
        }
    }

    //Explode on impact
    public void OnCollisionEnter(Collision c)
    {
        SetBoolExploded(true);
        if (Shake != null)
        {
            Shake.TriggerShake();
        }
        if (AreaDmg)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, Radius, damagable);

            foreach (Collider col in colliders)
            {
                Transform parentTransform = col.transform.parent != null ? col.transform.parent : col.transform;
                float distance = Vector3.Distance(transform.position, col.transform.position);
                ActualDmg = Mathf.Max((1 - distance / Radius) * dmg, 0);
                if (parentTransform.gameObject.TryGetComponent<AttributesManager>(out AttributesManager nearby))
                {
                    if (distance != 0f)
                    {
                        nearby.TakeDmg(ActualDmg);
                    }
                    else
                    {
                        nearby.TakeDmg(dmg);
                    }
                }
            }
            if (ExplosionFX != null && ExFxShow)
            {
                Instantiate(ExplosionFX, transform.position, Quaternion.identity);
            }
            if (ExplosionLight != null && ExFxShow)
            {
                Instantiate(ExplosionLight, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }

    public void SetBoolExploded(bool value)
    {
        Exploded = value;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Radius);
    }
}
