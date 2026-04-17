using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class JonsDogMovementController : MonoBehaviour
{
    private NavMeshAgent agent;
    [SerializeField] GameObject target;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.SetDestination(target.transform.position);
    }

    void Update()
    {
        if (target != null)
        {
            agent.SetDestination(target.transform.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
            target = null;
        }
    }
}
