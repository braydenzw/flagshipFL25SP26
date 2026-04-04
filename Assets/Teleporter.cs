using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Teleporter : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform teleportSpot; 
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            other.transform.position = teleportSpot;
        }
    }
    
}
