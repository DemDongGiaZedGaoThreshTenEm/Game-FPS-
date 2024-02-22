using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponWallChecking : MonoBehaviour
{
    public Transform GroundAndWallDetector;
    public LayerMask ground;
    public GameObject Gun;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool isWall = Physics.Raycast(GroundAndWallDetector.position, Vector3.forward, 5f, ground);
        if(isWall)
        {
            Gun.GetComponent<Animator>().Play("WallCheking");
        }

    }
}
