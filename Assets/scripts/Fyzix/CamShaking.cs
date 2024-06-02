using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamShaking : MonoBehaviour
{
    public float Intensity;
    public float ShakeDistance;
    public float duration;
    private float shakeTime;
    private float range;
    private Quaternion originalRotation;
    private bool randomizeX = false;
    private bool randomizeY = false;
    private bool randomizeZ = true;

    public Vector3 OriginalPos;
    private DmgManagement D;
    // Start is called before the first frame update
    void Start()
    {
        OriginalPos = transform.localPosition;
        originalRotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        GameObject[] ExplsSrc = GameObject.FindGameObjectsWithTag("Explosive");
        foreach (GameObject Expls in ExplsSrc)
        {
            D = Expls.GetComponent<DmgManagement>();
            if (D != null)
            {
                Debug.Log("Found Expls source");
            }
        }
        FindExplsSouce();
    }

    void FindExplsSouce()
    {
        GameObject[] ExplsVFXs = GameObject.FindGameObjectsWithTag("ExplosionVFXs");
        if (ExplsVFXs != null)
        {
            foreach (GameObject ExplsVFX in ExplsVFXs)
            {
                ExistingDuration existingDuration = ExplsVFX.GetComponent<ExistingDuration>();
                if (existingDuration != null)
                {

                    if (existingDuration.exploded == true)
                    {
                        Vector3 dir = ExplsVFX.transform.position - transform.position;
                        range = dir.magnitude;
                        ShakeDistance = D.ImpactLoud * 0.05f;
                        if (range <= ShakeDistance)
                        {

                            TriggerShake();

                        }

                    }

                }
            }
        }              
    }
    public void TriggerShake()
    {
        shakeTime = duration;
    }

    void LateUpdate()
    {
        if (shakeTime > 0f)
        {
            float adjustedIntensity = Intensity * (1 - Mathf.Pow(Mathf.Clamp01(range / ShakeDistance), 0.5f));
            
            Debug.Log("CurrentIntensity:" + adjustedIntensity);

            // Apply shake effect

            //transform.localPosition = OriginalPos + Random.insideUnitSphere * adjustedIntensity; (Vibrating mode: For large explosion, volcanic eruption,...)
            shakeTime -= Time.deltaTime;

            float randomX = randomizeX ? Random.Range(0f, 360f) * adjustedIntensity * Time.deltaTime : transform.eulerAngles.x;
            float randomY = randomizeY ? Random.Range(0f, 360f) * adjustedIntensity * Time.deltaTime : transform.eulerAngles.y;
            float randomZ = randomizeZ ? Random.Range(0f, 360f) * adjustedIntensity * Time.deltaTime : transform.eulerAngles.z;
            Quaternion targetRotation = Quaternion.Euler(
                originalRotation.eulerAngles.x + randomX * adjustedIntensity,
                originalRotation.eulerAngles.y + randomY * adjustedIntensity,
                originalRotation.eulerAngles.z + randomZ * adjustedIntensity
            );
            transform.rotation = Quaternion.Euler(randomX, randomY, randomZ);

        }
        else
        {
            // Reset to original position when shaking is done`

            //transform.localPosition = OriginalPos;
            transform.localRotation = Quaternion.Slerp(transform.localRotation, originalRotation, Time.deltaTime * 2f);
            shakeTime = 0f;
        }
    }
}
