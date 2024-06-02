using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributesManager : MonoBehaviour
{
    [SerializeField] public float HP;
    private float maxHP;
    public float HPRegen;
    [SerializeField] public float HPRegenDelay;

    [SerializeField] float STAM;
    private float maxSTAM;

    public float PW;
    float PwConsumption;
    [SerializeField] public float PWRegen;
    [SerializeField] public float PWRegenDelay;
    public float maxPW;
    public bool PWisRegenerating = false;
    bool AbilitiesUsed = false;


    [SerializeField] float Strength;

    [SerializeField] float Exp;
    private float maxExp;

    public GameObject Character;
    public GameObject Overshield;

    public Transform ShieldSpot;
    public bool ShieldBuffed;
    bool readyToBuff;
    public bool allowInvoke = true;

    public int BuffChance = 1;

    GunProjectile Pr;
    Abilities PwC;
    [SerializeField] float Dmg;
    private int i;
    public Guns ShieldingEffects;
    // Start is called before the first frame update
    void Start()
    {   
        GameObject Bullet = Pr.gameObject.GetComponent<GunProjectile>().bullet;
        Dmg = Bullet.GetComponent<DmgManagement>().dmg;
        Abilities ab = this.GetComponent<Abilities>();
        PwConsumption = ab.PWConsumption;
        maxPW = 200f;
        HP = maxHP ;
        STAM = maxSTAM ;
        PW = maxPW;
    }
    void Awake()
    {
        readyToBuff = true;
    }
    // Update is called once per frame
    void Update()
    {
        maxPW = 200f;
        maxHP = 200f;
        TakeDmg(Dmg);
        if(PW < maxPW) StartCoroutine(PWRegenerate());
        if(HP < maxHP) StartCoroutine(HPRegenerate());
        /*
        if (readyToBuff && BuffChance > 0)
        {
           OvershieldBuffed();
        }
        if(readyToBuff = false && ShieldBuffed == false)
        {
            Destroy(Overshield);
        }   */
       
    }

    public void TakeDmg(float value)
    {
        HP -= value;
        if(HP <= 0f)
        {
            HP = 0f;
            Destroy(this.gameObject);
        }    
    }

    public void Healing(float value)
    {
        HP += value;
        if (HP >= maxHP)
            HP = maxHP;
    }

    public void UseAbilities(float PWConsumption)
    {
        PW -= PWConsumption;
        PWisRegenerating = false;
        if(PWConsumption >= PwConsumption)
        {
            AbilitiesUsed = true;
        }    
    }

    private IEnumerator PWRegenerate()
    {
        PWisRegenerating = true;
        yield return new WaitForSeconds(PWRegenDelay);

        while (PW < maxPW || !PWisRegenerating )
        {
            PW += PWRegen * Time.deltaTime * 0.001f;
            yield return null;    
        }
        PW = Mathf.Clamp(PW, 0f, maxPW);
    }

    private IEnumerator HPRegenerate()
    {
        yield return new WaitForSeconds(PWRegenDelay);
        while (HP < maxHP)
        {
            HP += HPRegen * Time.deltaTime * 0.001f;
            yield return null;
        }
        if(AbilitiesUsed)
        {
            yield break;
        }    
        HP = Mathf.Clamp(HP, 0f, maxHP);
    }

    public void OvershieldBuffed()
    {
        ShieldBuffed=true;
        readyToBuff=false;
        //Applied Overshield 
        GameObject Ovrshld = Instantiate(Overshield, ShieldSpot, worldPositionStays: false);
           
        if(!ShieldBuffed && readyToBuff== false && BuffChance <=0)
        {
            Destroy(Ovrshld);
        }    
        // Dmg when Overshield is buffing;
        if (ShieldingEffects.ShieldPenetration == 0f)
        {
                Dmg = 0f;
        }
        if (ShieldingEffects.ShieldPenetration > 0f)
        {
            float realDmg = Dmg * (1f - ShieldingEffects.ShieldPenetration / 100f);
            TakeDmg(realDmg);
        }

        BuffChance--;
    }

    public void ReBuffOvrshld()
    {
        allowInvoke = true;
        ShieldBuffed = true;   
    }
    public void LvlUp(float ExpGained)
    {
        Exp += ExpGained;
        maxHP = maxHP + 20f;
        maxSTAM = maxSTAM + 40f;
        maxPW = maxPW + 15f;
        if (Exp >= maxExp)
            Exp = maxExp;
    }
}
