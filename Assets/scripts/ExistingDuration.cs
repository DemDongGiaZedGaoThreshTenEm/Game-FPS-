using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExistingDuration : MonoBehaviour
{
    public float duration;
    public bool exploded;

    private float explodeDuration = 0.5f; // Thời gian giữ exploded là true (ví dụ: 0.5 giây)

    private bool delayExplodedReset = true;

    // Start is called before the first frame update
    void Start()
    {
        // Set exploded to true immediately when the object is instantiated
        exploded = true;

        // Destroy the GameObject after the specified duration
        Destroy(gameObject, duration);

        // Schedule resetting exploded to false after a delay
    }

    void Update()
    {
        Invoke("ResetExploded", explodeDuration);
    }
    // Function to reset exploded to false after the delay
    private void ResetExploded()
    {
        exploded = false;
    }
}
