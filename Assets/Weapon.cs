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

    #endregion
    #region MonoBehaviour
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Equip(0);
            Gun.GetComponent<Animator>().Play("Weapon - taking or swapping");
        }
        if (CurrentWeapon!=null)
        {
            aim(Input.GetMouseButton(1));
        }
    }
    #endregion
    #region Method
    void Equip(int p_ind)
    {
        if (CurrentWeapon != null)
            Destroy(CurrentWeapon);
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

        if(p_aim)
        {
            Anchor.position = Vector3.Lerp(Anchor.position, Status_ads.position, Time.deltaTime * LoadOut[Index].AimSpeed);
        }
        else
        {
            Anchor.position = Vector3.Lerp(Anchor.position, Status_hip.position, Time.deltaTime * LoadOut[Index].AimSpeed);

        }
    }
    #endregion
}
