using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// author: Ethan Roberts
/// if it's a buncha junk, sorry mate
/// 
/// 
/// </summary>
public class EnemyMovementControllerRevamped : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform target; // this is what the enemy is set to pursue
    private Transform player;
    private enum states
    {
        searching,
        chasing,
        wandering
    }

    // use to define behavior categories
    private states state = states.searching;

    // parameters
    private float chase_time = 5;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.Log("enemy does not have a NavMeshAgent attached");
        }

        player = GameObject.FindWithTag("Player").transform;

        state = states.wandering;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case states.searching:
                break;

            case states.wandering:
                
                break;

            case states.chasing:
                StartCoroutine(ChasePlayer());
                break;

            default:
                Debug.Log("enemy invalid state (logic prob scuffed)");
                break;
        }

        if (target != null)
        {
            agent.SetDestination(target.position);
        }
    }

    void setTargetToPlayer(Transform player)
    {
        if (player == null)
        {
            Debug.Log("enemy could not find player; check if player is tagged");
        }

        target = player;
    }


    // returns a target vector 
    private Vector3 Wander(float radius)
    {
        // find ran dir in unit sphere, scale by radius
        Vector3 dir = Random.insideUnitSphere * radius;
        // make dir relative to agent position
        dir += agent.transform.position;
        NavMeshHit hit;
        Vector3 target = Vector3.zero;
        
        // get the closest point on the NavMesh
        if (NavMesh.SamplePosition(dir, out hit, radius, NavMesh.AllAreas))
        {
            target = hit.position;
        }

        return target;
    }

    private IEnumerator ChasePlayer() {
        Debug.Log("enemy started chasing player");
        setTargetToPlayer(player);
        yield return new WaitForSeconds(chase_time);
        target = null;
        state = states.wandering;
    }
}
