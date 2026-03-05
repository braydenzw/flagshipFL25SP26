using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// REALLY basic script that turns the flashlight on and off
// attach this to the Light object that will act as the flashlight
public class PlayerFlashlight : MonoBehaviour
{
    private Light flashlight;
    private bool usable = false;
    [SerializeField] AudioClip flashlightClip;

    private void Awake()
    {
        flashlight = GetComponent<Light>();
    }

    private void Start()
    {
        StartCoroutine(flashlightStart());
    }

    // Update is called once per frame
    void Update()
    {
        if (usable)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                Debug.Log("FLASHLIGHT TOGGLE");
                toggleFlashlight();
            }
        }
    }

    void toggleFlashlight()
    {
        flashlight.enabled = !flashlight.enabled;
        SoundManager.instance.PlaySound(flashlightClip, transform, 1f);
    }

    IEnumerator flashlightStart()
    {
        yield return new WaitForSeconds(3);
        toggleFlashlight();
        usable = true;
        yield return null;
    }
}
