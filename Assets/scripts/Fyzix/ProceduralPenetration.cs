using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralPenetration : MonoBehaviour
{
    private Rigidbody rb;
    private RaycastHit[] hits;
    public DmgManagement DmgScript;
    public float PenetratingPower;

    void Awake()
    {
        PenetratingPower = this.DmgScript.PenetratingPower;
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        DmgScript = this.GetComponent<DmgManagement>();
    }

    void Update()
    {
        Vector3 direction = transform.forward;
        float distance = rb.velocity.magnitude * Time.fixedDeltaTime;
        int layerMask = ~(1 << LayerMask.NameToLayer("Projectile")); // Exclude "Projectile" layer

        hits = rb.SweepTestAll(direction, distance, QueryTriggerInteraction.Ignore);

        if (hits.Length > 0)
        {
            // Sort hits by distance
            System.Array.Sort(hits, (h1, h2) => h1.distance.CompareTo(h2.distance));

            Vector3 currentPosition = transform.position;
            foreach (RaycastHit hit in hits)
            {
                GameObject hitTarget = hit.collider.gameObject;

                if (hitTarget == gameObject)
                {
                    continue;
                }

                Debug.Log($"Hit object: {hitTarget.name}");
                float penetration = CalculateDepth(hitTarget);
                Debug.Log($"Hit Target: {hitTarget.name}, Penetration: {penetration}");

                if (penetration >= 0)
                {
                    DmgScript.PenetratingPower = penetration;
                    // Optionally apply damage to the hit target
                    PenetratingPower = penetration;
                    // Update the bullet's position to the exit point of the current object
                    currentPosition = hit.point + direction * 0.01f;
                    transform.position = currentPosition;
                }
                else
                {
                    Destroy(gameObject);
                    Debug.Log("Bullet Destroyed");
                    break;
                }
            }
        }
    }

    public float CalculateDepth(GameObject target)
    {
        if (target.TryGetComponent<PenetratingResist>(out PenetratingResist resist))
        {
            Debug.Log("Bullet Impacted");
            float TargetHardness = resist.PntrResistance;
            float TargetThickness = target.GetComponent<Renderer>().bounds.size.magnitude;

            float remainingPenetrationPower = DmgScript.PenetratingPower - (TargetHardness * TargetThickness);

            if (remainingPenetrationPower > 0)
            {
                return remainingPenetrationPower;
            }
            else
            {
                return -1; // Not enough penetrating power to penetrate
            }
        }

        return DmgScript.PenetratingPower; // No resistance component found, assume full penetration
    }
}
