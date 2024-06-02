using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    #region Variables
    public Guns[] LoadOut;
    private int Index;
    public Transform WeaponParent;
    private GameObject CurrentWeapon;
    public GameObject Gun;
    public bool isAiming = false;
    public bool equipped = false;
    #endregion
    #region MonoBehaviour
    // Update is called once per frame
    void Update()
    {
        // Check if the player is aiming
        bool isAiming = Input.GetMouseButton(1);

        // If the player is not aiming, allow weapon switching
        if (Index < LoadOut.Length && Index >= 0)
        {
            if (!isAiming)
            {
                // Check if the player presses the 1 key to equip another weapon

                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    equipped = true;
                    Equip(0);
                    Gun.GetComponent<Animator>().Play("Weapon - taking or swapping");
                }

            }
        }
        else
        {
            Destroy(CurrentWeapon);
            equipped = false;
        }
        
           
            // Update the aim based on whether the player is aiming or not
            if (CurrentWeapon != null)
        {
            aim(isAiming);
        }
    }
    #endregion
    #region Method
    void Equip(int p_ind)
    {

        if (CurrentWeapon != null)
        {
            Destroy(CurrentWeapon);          
        }

        Index = p_ind;
        GameObject newWeapon = Instantiate(LoadOut[p_ind].prefabs, WeaponParent.position, WeaponParent.rotation, WeaponParent) as GameObject;
        newWeapon.transform.localPosition = Vector3.zero;
        newWeapon.transform.localEulerAngles = Vector3.zero;

        CurrentWeapon = newWeapon;
    }
    void aim(bool p_aim)
    {
        Transform Anchor = CurrentWeapon.transform.GetChild(0);
        Transform Status_hip = CurrentWeapon.transform.GetChild(1).GetChild(0);
        Transform Status_ads = CurrentWeapon.transform.GetChild(1).GetChild(1);
        if(!Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (p_aim)
            {
                isAiming = true;
                Anchor.position = Vector3.Lerp(Anchor.position, Status_ads.position, Time.deltaTime * LoadOut[Index].AimSpeed);
            }
            if (!p_aim)
            {
                Anchor.position = Vector3.Lerp(Anchor.position, Status_hip.position, Time.deltaTime * LoadOut[Index].AimSpeed);
                isAiming = false;
            }

        }
    }
    #endregion
}
