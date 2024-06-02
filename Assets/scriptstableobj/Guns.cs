using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Gun", menuName = "Gun")]
public class Guns : ScriptableObject
{
    public GameObject prefabs;
    // General stats
    public string Name;
    [Header("Weapon Types")]
    public bool BoltActionSniper;
    public bool SemiAuto;
    public bool Automatic;
    public bool PumpActionShotGun;

    public float firerate;
    public float mobility;
    public float AimSpeed;
    public float Damage;
    public float MuzzleBlastLoud;
    [Header("Dmg Types")]
    public bool Projectile;
    public bool Melee;
    public bool Explosive;
    public bool Incendiary;
    public bool Laser;
    [Header("Percentage")]
    [Range(0,1)]
    public float MobilityRatio;

    [Range(0, 100)]
    public bool Critical;

    public float CritDmg;

    [Range(0, 100)]
    public float CritRate;

    // Special/Unique stats
    [Range(0, 100)]
    [SerializeField]public float ShieldPenetration;
     private float maxShieldPenetration = 100f;
    [Range(0, 100)]
    [SerializeField]public float ShieldBusting;
     private float maxShieldBusting = 100f;

    // Start is called before the first frame update
    public void Start()
    {
        if (ShieldPenetration >= maxShieldPenetration)
            ShieldPenetration = maxShieldPenetration;
        if (ShieldBusting >= maxShieldBusting)
            ShieldBusting = maxShieldPenetration;
        if (Critical == false)
        {
            CritDmg = 0f;
            CritRate = 0f;
        }    
    }
}
