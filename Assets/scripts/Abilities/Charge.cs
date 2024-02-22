using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charge : MonoBehaviour
{
    [SerializeField] public Abilities ability;
    [SerializeField] public float Pw;

    [SerializeField] public float ChargeSpeed = 10f;
    [SerializeField] public float inputDelay;
    
    public GameObject Player;
    public Transform initialPos;

    public bool readyToCast;
    public bool Casting;
    public bool allowInvoke = true;
    //FOV modifying
    public Camera normalCam;
    public bool isCharging = false;
    public float movementSpd = 125;
    private Rigidbody rb;
    float FOV;//flied of view
    public float ChargeFOVModifier;

    // Start is called before the first frame update
    void Start()
    {
        rb = Player.GetComponent<Rigidbody>();
        FOV = normalCam.fieldOfView;
        //Camera.main.enabled = false;

    }
    private void Awake()
    {
        readyToCast = true;
    }

    // Update is called once per frame
    void Update()
    {
        
        Pw = this.gameObject.GetComponentInParent<AttributesManager>().PW;
        MyInput();
    }

    void MyInput()
    {
        Casting = Input.GetKeyDown(KeyCode.E);
        if(Casting && readyToCast && ability.PWConsumption <= Pw && Pw > 0)
        {
            StartCoroutine(Cast());
        }    
    }

    IEnumerator Cast()
    {
        readyToCast = false;
        
        yield return new WaitForSeconds(inputDelay);
        float elapsedTime = 0f;
        //duration of 1 charge
        while (elapsedTime < 100f)
        {
            elapsedTime += ChargeSpeed * Time.fixedDeltaTime;
            
            //Add forces 2 Charge
            rb.AddForce((initialPos.position - transform.position).normalized * ChargeSpeed * 50f, ForceMode.Force);
            //Apply FOV change while charging
            normalCam.fieldOfView = Mathf.Lerp(normalCam.fieldOfView, FOV * ChargeFOVModifier, Time.deltaTime * 8f);          
            yield return null;
        }
        //Player's velocity must be reset after dash
        ResetVelocity();
        //Cooldown
        if (allowInvoke)
        {
            Invoke("ResetCasting", ability.Cooldown);
            allowInvoke = false;
        }   
        AttributesManager a = this.gameObject.GetComponentInParent<AttributesManager>();
        a.UseAbilities(ability.PWConsumption);
    }

    void ResetCasting()
    {
        readyToCast = true;
        allowInvoke = true;
    }

    public void ResetVelocity()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}
