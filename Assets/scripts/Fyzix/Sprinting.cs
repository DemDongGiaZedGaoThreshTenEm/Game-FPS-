using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprinting : MonoBehaviour
{
    public Camera normalCam;
    public bool isMoving = false;
    public float movementSpd = 125;
    public float CurrentSpeed;

    private Rigidbody rb;
    float FOV;//flied of view
    float SprintFOVModifier = 1.5f;
    public Transform GroundDetector;
    public LayerMask ground;
    public GameObject Gun;
    public Guns CurrentWeapon;

    // Start is called before the first frame update
    void Start()
    {
        FOV = normalCam.fieldOfView;
        //Camera.main.enabled = false;
        CurrentSpeed = movementSpd;
    }

    // Update is called once per frame
    void Update()
    {
        Weapon wp = this.GetComponent<Weapon>();
        CurrentWeapon = wp.LoadOut[0];
        if (CurrentWeapon != null && wp.equipped)
        {
            CurrentSpeed = CurrentWeapon.MobilityRatio * movementSpd;
        }

        bool isGround = Physics.Raycast(GroundDetector.position, Vector3.down, 0.1f, ground);
        
        if (Input.GetKey(KeyCode.W))
        {
            isMoving = true;
        }

        if (Input.GetKeyUp("w") || (Input.GetKeyUp("w") && Input.GetKeyUp(KeyCode.LeftShift)))
        {
            isMoving=false;
            Gun.GetComponent<Animator>().Play("New State");          
        }

        if (Input.GetKey(KeyCode.LeftShift) & isMoving == true &  isGround && !Input.GetMouseButton(1) && !Input.GetKey(KeyCode.Space))
        {
            normalCam.fieldOfView = Mathf.Lerp(normalCam.fieldOfView, FOV * SprintFOVModifier, Time.deltaTime * 3f);
            transform.position += transform.forward * Time.deltaTime * CurrentSpeed;
            Gun.GetComponent<Animator>().Play("Weapon - Running State");
            //ani.SetFloat("Speed", );
        }  
        else
        {
            normalCam.fieldOfView = Mathf.Lerp(normalCam.fieldOfView, FOV, Time.deltaTime * 8f);
        }
        
    }
}
