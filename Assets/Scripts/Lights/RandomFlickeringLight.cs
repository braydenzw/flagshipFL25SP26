using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomFlickeringLight : FlickeringLight
{
    [Header("use these instead:")]
    [SerializeField] float minInterval;
    [SerializeField] float maxInterval;

    protected override IEnumerator Flicker()
    {
        flickering = true;
        setIntensity(highIntensity);
        yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));
        setIntensity(lowIntensity);
        yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));
        flickering = false;
    }
}
