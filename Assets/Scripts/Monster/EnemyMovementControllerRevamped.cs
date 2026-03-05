using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; 

// author: Ethan Roberts
// if it's a buncha junk, I apologize in advance
public class EnemyMovementControllerRevamped : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform target; 

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.Log("enemy does not have a NavMeshAgent attached");
        }

        // set target to player
        target = GameObject.FindWithTag("Player").transform;
        if (target == null)
        {
            Debug.Log("enemy could not find player");
        }
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(target.position);
    }
}
