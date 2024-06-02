using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Ability", menuName = "Ability")]
public class Abilities : ScriptableObject
{
    public GameObject CastPoint;
    [SerializeField] public float Cooldown;
    [SerializeField] public float Duration;
    [SerializeField] public float PWConsumption;


    // Start is called before the first frame update
    void Start()
    {

    }
}



