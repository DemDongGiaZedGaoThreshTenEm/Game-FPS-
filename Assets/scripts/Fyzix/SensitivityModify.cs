using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensitivityModify : MonoBehaviour
{
    public Rotation rot;
    [SerializeField] public float normalRotX;
    [SerializeField] public float normalRotY;

    public float XRotModified;
    public float YRotModified;  
    public Weapon wp;
    // Start is called before the first frame update
    void Awake()
    {
    }
    void Start()
    {
        rot = GetComponentInParent<Rotation>();
        wp = GetComponentInParent<Weapon>();
        normalRotX = rot.XSensitivity;
        normalRotY = rot.YSensitivity;

    }
    // Update is called once per frame
    void Update()
    {

        bool isAiming = GetComponentInParent<Weapon>().isAiming;
        if (!isAiming)
        {
            rot.XSensitivity = normalRotX;
            rot.YSensitivity = normalRotY;  
        }

        if (isAiming && !Input.GetKeyDown(KeyCode.Alpha1))
        {
            rot.XSensitivity = XRotModified;
            rot.YSensitivity = YRotModified;
        }    
        
    }
}
