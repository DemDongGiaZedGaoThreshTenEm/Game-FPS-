using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeWarpEffect : MonoBehaviour
{
    private float originalTimeScale = 1f;

    public void ApplyTimeWarp(float timeWarpFactor, float duration)
    {
        StartCoroutine(TimeWarp(timeWarpFactor, duration));
    }

    private IEnumerator TimeWarp(float timeWarpFactor, float duration)
    {
        originalTimeScale = Time.timeScale; // Store the original global time scale
        Time.timeScale = timeWarpFactor; // Apply the new time scale globally (this is necessary for proper wait times)
        float endTime = Time.realtimeSinceStartup + duration * timeWarpFactor;

        while (Time.realtimeSinceStartup < endTime)
        {
            // Here you can update object-specific behaviors if needed
            yield return null;
        }

        Time.timeScale = originalTimeScale; // Reset the global time scale    }
    }
    public void ResetTimeScale()
    {
        Time.timeScale = originalTimeScale;
    }
}
