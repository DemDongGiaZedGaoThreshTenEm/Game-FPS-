using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingDmg : MonoBehaviour
{
    [SerializeField] public float DmgMultiplier;
    [SerializeField] public float NoDmgVel;
    public float LastFallVel;
    float dmgCoolDown = 1f;
    public Rigidbody rb;
    public LayerMask ground;
    bool wasGrounded;
    public Transform GroundDetector;
    bool canTakeDamage = true;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //reset fall vel if is grounded
        if(!isGround() && wasGrounded)
        {
            LastFallVel = 0f;
        }
        wasGrounded = isGround();
    }
    void FixedUpdate()
    {
        UpdateVelocity();
    }

    void UpdateVelocity()
    {
        // Calculate the current fall velocity
        float currentFallVelocity = -rb.velocity.y;

        // Update the last fall velocity if the current fall velocity is greater
        LastFallVel = Mathf.Max(LastFallVel, currentFallVelocity);
          
    }
    void OnCollisionEnter(Collision c)
    {
        if (ground == (ground | (1 << c.gameObject.layer)))
        {
            if (LastFallVel > NoDmgVel && canTakeDamage)
            {
                float impactAngle = Vector3.Angle(Vector3.up, c.GetContact(0).normal);
                float currentDmg = DmgMultiplier * (LastFallVel - NoDmgVel) * Mathf.Clamp01(impactAngle / 90f);
                if (c.gameObject.CompareTag("water"))
                {
                    currentDmg = 0f;
                }

                AttributesManager A = this.gameObject.GetComponent<AttributesManager>();
                A.TakeDmg(currentDmg);
                StartCoroutine(DmgCoolDown());
                
                LastFallVel = 0f;
            }
                
        }

        
    }

    private IEnumerator DmgCoolDown()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(dmgCoolDown);
        canTakeDamage = true;
    }

    bool isGround()
    {
        return Physics.Raycast(GroundDetector.position, Vector3.down, 0.1f, ground);
    }

}
