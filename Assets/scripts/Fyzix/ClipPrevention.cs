using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClipPrevention : MonoBehaviour
{
    public Animator anim;
    public GameObject ClipProjector;
    public float checkDistance;
    public float reCheckDistance;
    public Vector3 newDr, newDr2;
    public Transform newGunPos;
    float lerpPos;
    RaycastHit hit;
    public LayerMask ground;
    public LayerMask NoClip;
    Vector3 initialGunPos;

    // Start is called before the first frame update
    void Start()
    {
        // Lưu trữ vị trí ban đầu của súng
        initialGunPos = newGunPos.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        anim = this.GetComponent<Animator>();

        reCheckDistance = -checkDistance;
        Mathf.Clamp01(lerpPos);
        bool noClip = Physics.Raycast(ClipProjector.transform.position, -ClipProjector.transform.forward, out hit, checkDistance, NoClip);

        // Kiểm tra va chạm và điều chỉnh vị trí súng
        if (Physics.Raycast(ClipProjector.transform.position, -ClipProjector.transform.forward, out hit, checkDistance) && !(Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W)) && !noClip)
        {
            anim.updateMode = AnimatorUpdateMode.AnimatePhysics;
            lerpPos = 1 - (Mathf.Abs(hit.distance) / checkDistance);
            transform.localRotation = Quaternion.Slerp(Quaternion.Euler(Vector3.zero), Quaternion.Euler(newDr2), lerpPos);

            // Điều chỉnh vị trí của súng
            Vector3 adjustedGunPos = initialGunPos - new Vector3(0, 0, -lerpPos * hit.distance * 0.7f);
            newGunPos.localPosition = Vector3.Lerp(newGunPos.localPosition, adjustedGunPos, lerpPos);
        }
        // Khi di chuyển và không có va chạm
        /*else if (Physics.Raycast(ClipProjector.transform.position, ClipProjector.transform.forward, out hit, checkDistance) && Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W))
        {
            lerpPos = 1 - (Mathf.Abs(hit.distance) / checkDistance);
            transform.localRotation = Quaternion.Slerp(Quaternion.Euler(Vector3.zero), Quaternion.Euler(newDr), lerpPos);

            // Điều chỉnh vị trí của súng
            Vector3 adjustedGunPos = initialGunPos - new Vector3(0, 0, -lerpPos * hit.distance * 0.5f);
            newGunPos.localPosition = Vector3.Lerp(newGunPos.localPosition, adjustedGunPos, lerpPos);
        }*/
        else
        {
            lerpPos = 0;
            // Trả súng về vị trí ban đầu khi không có va chạm
            newGunPos.localPosition = Vector3.Lerp(newGunPos.localPosition, initialGunPos, Time.deltaTime * 2f); // Tốc độ trở lại có thể điều chỉnh
        }
    }
}
