using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

// if an object enters this trigger collider,
// it will switch the active status of another object

// e.g. if player walks into the collider, do something
// e.g. if player places a specific object into this collider, do something
public class ActivatorTriggerCol : MonoBehaviour
{
    // when obj_to_enter enters this collider, 
    // it will switch the active status of objs_to_be_activated
    [SerializeField] GameObject obj_to_enter;
    [SerializeField] List<GameObject> objs_to_be_activated = new List<GameObject>();
    [SerializeField] AudioClip clip; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == obj_to_enter)
        {
            if (clip != null)
            {
                SoundManager.instance.PlaySound(clip, transform, 1f);
            }

            foreach (GameObject obj in objs_to_be_activated)
            {
                obj.SetActive(!obj.activeSelf);
            }

            Destroy(gameObject);
        }
    }
}
