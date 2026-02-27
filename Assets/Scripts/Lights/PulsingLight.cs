using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulsingLight : FlickeringLight
{   
    override protected IEnumerator Flicker()
    {
        flickering = true;
        l.intensity += 0.25f;
        yield return new WaitForSeconds(flickerInterval);
        flickering = false;
    }
}
