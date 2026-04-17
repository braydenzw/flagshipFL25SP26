using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// designed to work with a parent with EnemyMovementControllerRevamped script
public class BraydenDetectionObj : MonoBehaviour
{
    BraydenMovementControllerRevamped enemy_script;

    private void Awake()
    {
        enemy_script = GetComponentInParent<BraydenMovementControllerRevamped>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemy_script.foundPlayer();
        }
    }
}
