using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// instantiates SoundObjects to play SFX
// based on https://www.youtube.com/watch?v=DU7cgVsU2rM

// to use:
// assign SFX to the gameObject you want to play SFX as a local var
// call SoundManager.instance.PlaySound(<SFXname>, transform, 1f);
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField] AudioSource soundObject;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // instantiates soundObject to play SFX, then destroys itself after SFX is done playing
    // volume controls how loud the SFX is relative to the original volume
    public void PlaySound(AudioClip clip, Transform spawn, float volume)
    {
        AudioSource source = Instantiate(soundObject, spawn.position, Quaternion.identity);

        source.clip = clip;
        source.volume = volume;
        source.Play();
        float clipLength = source.clip.length;
        Destroy(source.gameObject, clipLength);
    }
}
