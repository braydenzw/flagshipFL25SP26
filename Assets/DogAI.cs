using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DogAI : MonoBehaviour
{
    public string enemyTag = "Enemy";
    public float attackRange = 2f;
    public float jumpForce = 5f;
    
    private GameObject targetEnemy;
    private NavMeshAgent agent;
    private bool hasJumped = false;

 
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 1.0f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position); 
        }

        FindClosestEnemy();
    }

    void Update()
    {
        if (targetEnemy == null) return;

        if (agent.isActiveAndEnabled && agent.isOnNavMesh)
        {

            Vector3 direction = (targetEnemy.transform.position - transform.position).normalized;
            direction.y = 0; 
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
            }

            float distance = Vector3.Distance(transform.position, targetEnemy.transform.position);

            if (distance > attackRange)
            {
                if (agent.velocity.sqrMagnitude < 0.1f) 
                {
                    agent.SetDestination(targetEnemy.transform.position);
                }
            }
            else if (!hasJumped)
            {
                AttackEnemy();
            }
        }
    }

    void FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float closestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                targetEnemy = enemy;
            }
        }
    }

    void AttackEnemy()
    {
        hasJumped = true;
        agent.isStopped = true; // Stop walking to jump

        // Physical "Jump" toward enemy
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 direction = (targetEnemy.transform.position - transform.position).normalized;
            rb.AddForce((direction + Vector3.up) * jumpForce, ForceMode.Impulse);
        }

        // Tell the enemy to die
        EnemyMovementController enemyScript = targetEnemy.GetComponent<EnemyMovementController>();
        if (enemyScript != null)
        {
            enemyScript.Die();
        }
    }
}