using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartBlackHole : MonoBehaviour
{
    public AbilitiesManager AbMng;
    [SerializeField] public Abilities ability;
    [SerializeField] public float Pw;
    
    [SerializeField] public float speed;
    [SerializeField] public float inputDelay;

    public Transform Center;
    public Transform attackPoint;
    public GameObject BlackHole;
    public bool readyToCast;
    public bool Casting;
    public int Activated;
    public int Amount;
    public bool allowInvoke = true;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Awake()
    {      
        readyToCast = true;
    }

    // Update is called once per frame
    void Update()
    {
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
        if(AbMng != null)
        {
            if ((ability == AbMng.Q_Ability && Input.GetKeyDown(KeyCode.Q))
             || (ability == AbMng.E_Ability && Input.GetKeyDown(KeyCode.E)))
            {
                if (readyToCast && ability.PWConsumption <= Pw && Pw > 0)
                {
                    Activate();
                }
            }
        }
    }

    public IEnumerator Cast()
    {
        readyToCast = false;
        //Add forces 2 bullet
        yield return new WaitForSeconds(inputDelay);
        Shoot();

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

    void Shoot()
    {
        GameObject BH = Instantiate(BlackHole, attackPoint.position, BlackHole.transform.rotation);
        Rigidbody rig = BH.GetComponent<Rigidbody>();
        rig.useGravity = false;
        rig.AddForce(attackPoint.forward * speed, ForceMode.Impulse);
        BH.GetComponent<Rigidbody>().AddForce(attackPoint.forward * speed, ForceMode.Impulse);
    }

        // Optionally, you can disable or destroy the object after implosion
        // obj.gameObject.SetActive(false);
        // or
        // Destroy(obj.gameObject);
    
}

