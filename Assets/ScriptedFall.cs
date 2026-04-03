using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ScriptedFall : MonoBehaviour
{
    [SerializeField] private float fallTimeSeconds = 60f;
    [SerializeField] TimeTravelManager timeManager;
    private Rigidbody rb;
    
    // To store initial state for time travel resets
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private bool hasFallen = false;
    // Start is called before the first frame update
    void Start()
    {
        
        rb = GetComponent<Rigidbody>();
        timeManager = FindObjectOfType<TimeTravelManager>();
        
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {

        float gameTime = timeManager.getCurrentTime();

        if (gameTime >= fallTimeSeconds && !hasFallen)
        {
            TriggerFall();
        }
        else if (gameTime < fallTimeSeconds && hasFallen)
        {
            ResetToWall();
        }
    }

    void TriggerFall()
    {
        rb.isKinematic = false; // Let gravity take over
        hasFallen = true;

        transform.position += transform.forward * 0.01f; //i think it clips the wall a bit this stuff just kinda pushes it forward and turns it so it's not like a straight drop
        Vector3 pushForce = transform.forward * 2f; 
        rb.AddForce(pushForce, ForceMode.Impulse);
        rb.AddTorque(new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1)), ForceMode.Impulse);
        Debug.Log("fall");
    }

    void ResetToWall()
    {
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        hasFallen = false;
    }
    
    
    [Header("Dog Spawn Settings")]
    [SerializeField] private GameObject dogPrefab;
    [SerializeField] private Transform spawnPoint; 
    private bool dogHasSpawned = false;


    private void OnCollisionEnter(Collision collision)
    {

        if (!hasFallen && collision.gameObject.CompareTag("Muffin") && !dogHasSpawned) //bad codei know
        {
            SpawnDog();
        }
    }

    void SpawnDog()
    {
        if (dogPrefab != null)
        {
           
            Instantiate(dogPrefab, spawnPoint.position, spawnPoint.rotation);
            dogHasSpawned = true;
            Debug.Log("JON'S DOG!");
        }
        else
        {
            Debug.LogWarning("fuck you");
        }
    }
}
