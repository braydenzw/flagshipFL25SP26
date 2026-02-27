using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// attach to a Light gameObject
// used for a flickering light that flickers at a constant rate
public class FlickeringLight : MonoBehaviour
{
    protected Light l;
    [SerializeField] protected float lowIntensity = 0f;
    [SerializeField] protected float highIntensity = 2f;
    [Header("if this is a RandomFlickeringLight script, ignore this:")]
    [Header("if this a PulsingLight script, DON'T ignore this:")]
    [SerializeField] protected float flickerInterval = 5f;
    protected bool flickering = false;

    private void Awake()
    {
        l = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!flickering)
        {
            StartCoroutine(Flicker());
        }
    }

    protected void setIntensity(float newIntensity)
    {
        l.intensity = newIntensity; 
    }

    protected virtual IEnumerator Flicker()
    {
        flickering = true;
        setIntensity(highIntensity);
        yield return new WaitForSeconds(flickerInterval);
        setIntensity(lowIntensity);
        yield return new WaitForSeconds(flickerInterval);
        flickering = false;
    }
}
