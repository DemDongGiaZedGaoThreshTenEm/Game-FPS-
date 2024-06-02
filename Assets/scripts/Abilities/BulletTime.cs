using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTime : MonoBehaviour
{
    public AbilitiesManager AbMng;
    [SerializeField] public Abilities ability;
    [SerializeField] public float Pw;
    [SerializeField] public float TimeWarpFactor;
    
    public Transform Player;
    public LayerMask Enemies;
    public float Radius;

    public bool readyToCast;
    public bool Casting;
    public bool allowInvoke = true;
    //FOV modifying
    public Camera normalCam;
    float FOV;
    public float ChargeFOVModifier;
    // Start is called before the first frame update
    void Start()
    {
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
        AbMng = GetComponentInParent<AbilitiesManager>();
        Pw = this.gameObject.GetComponentInParent<AttributesManager>().PW;
        MyInput();
    }

    public void Activate()
    {
        Cast();
        Debug.Log("Skill Activated");
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
                }
            }
        }
    }

    public void Cast()
    {
        readyToCast = false;
        //Ability Logic
        StartCoroutine(TimeWarp());
        //Cooldown
        if (allowInvoke)
        {
            Invoke("ResetCasting", ability.Cooldown * TimeWarpFactor);
            allowInvoke = false;
        }
        AttributesManager a = this.gameObject.GetComponentInParent<AttributesManager>();
        a.UseAbilities(ability.PWConsumption);
    }

    public IEnumerator TimeWarp()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, Radius, Enemies);
        List<TimeWarpEffect> affectedObjects = new List<TimeWarpEffect>();

        foreach (Collider collider in colliders)
        {
            Transform parentTransform = collider.transform.parent != null ? collider.transform.parent : collider.transform;
            if (parentTransform.CompareTag("Enemy"))
                {
                    TimeWarpEffect timeWarpEffect = parentTransform.gameObject.GetComponent<TimeWarpEffect>();

                    if (timeWarpEffect == null)
                    {
                        timeWarpEffect = parentTransform.gameObject.AddComponent<TimeWarpEffect>();
                    }

                    timeWarpEffect.ApplyTimeWarp(TimeWarpFactor, ability.Duration);
                    affectedObjects.Add(timeWarpEffect);
                }
        }
        yield return new WaitForSeconds(ability.Duration * TimeWarpFactor);
        Debug.Log("Skill Expired");

        foreach (TimeWarpEffect enemy in affectedObjects)
        {
            enemy.ResetTimeScale();
        }
        /*Time.timeScale = TimeWarpFactor;
        Debug.Log("Time Warpped");
        yield return new WaitForSeconds(ability.Duration);

        Time.timeScale = 1f;*/
    }

    private bool IsTeammate(Transform objTransform)
    {
        // Add logic to determine if the object is a teammate
        // For example, check for a specific tag or component
        return objTransform.CompareTag("Teammate");
    }

    private bool IsValidTarget(Transform objTransform)
    {
        // Add additional logic to determine if the object is a valid target
        // For example, exclude non-enemy NPCs, environmental objects, etc.
        return true;
    }

    void ResetCasting()
    {
        readyToCast = true;
        allowInvoke = true;
    }
}
