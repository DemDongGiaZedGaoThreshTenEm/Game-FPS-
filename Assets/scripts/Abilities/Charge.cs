using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charge : MonoBehaviour
{
    public GunProjectile GP;
    public GameObject CastingVFXs;
    public AbilitiesManager AbMng;
    [SerializeField] public Abilities ability;
    [SerializeField] public float Pw;

    [SerializeField] public float ChargeSpeed = 10f;
    [SerializeField] public float inputDelay;
    public float ElapsedTime;
    
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
    float FOV;
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
        GameObject parentObj = GameObject.FindWithTag("MainCamera");
        if (parentObj != null)
        {
            // Lấy component từ một đối tượng trong hierarchy cùng parent
            GunProjectile G = parentObj.GetComponentInChildren<GunProjectile>();

            if (G != null)
            {
                GP = G;
            }
            else
            {
                GP = G;
            }
        }
        else
        {
            Debug.LogWarning("Không tìm thấy đối tượng cha.");
        }
        AbMng = GetComponentInParent<AbilitiesManager>();
        Pw = this.gameObject.GetComponentInParent<AttributesManager>().PW;
        
        MyInput();
        
    }

    public void Activate()
    {

        StartCoroutine(Cast());
    }

    public void MyInput()
    {
        if (AbMng != null)
        {           
            if ((ability == AbMng.Q_Ability && Input.GetKeyDown(KeyCode.Q))
                || (ability == AbMng.E_Ability && Input.GetKeyDown(KeyCode.E)))
            {
                if (readyToCast && ability.PWConsumption <= Pw && Pw > 0)
                {
                    
                    Activate();
                    
                    ResetVelocity();
                }
            }    
        }
    }

    public IEnumerator Cast()
    {
        readyToCast = false;
        
        yield return new WaitForSeconds(inputDelay);
        float elapsedTime = 0f;
        //duration of 1 charge
        while (elapsedTime < ElapsedTime)
        {
            isCharging = true;
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
