using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// This script handles chasing enemies, jumping to attack, and 
// automatically disabling itself when picked up (parented).
public class DogAI : MonoBehaviour
{
    [Header("Targeting Settings")]
    public string enemyTag = "Enemy";
    public float attackRange = 2f;
    
    [Header("Attack Settings")]
    public float jumpForce = 6f;
    public float missRecoveryTime = 1.5f; // Seconds to wait before trying again

    private GameObject targetEnemy;
    private NavMeshAgent agent;
    private Rigidbody rb;
    private bool isAttacking = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        // Start kinematic so physics doesn't fight the NavMesh initially
        if (rb != null) rb.isKinematic = true;

        FindClosestEnemy();
    }

    void Update()
    {
        // 1. AUTOMATIC PICKUP DETECTION
        // If the dog has a parent (it's being held), stop all AI logic
        if (transform.parent != null)
        {
            if (agent.enabled) agent.enabled = false;
            if (rb != null) rb.isKinematic = true;
            isAttacking = false; 
            StopAllCoroutines();
            return; 
        }

        // 2. DROP DETECTION
        // If the dog was just dropped, re-enable the NavMesh
        if (transform.parent == null && !agent.enabled && !isAttacking)
        {
            ResetToNavMesh();
        }

        // 3. AI BEHAVIOR
        if (targetEnemy == null) 
        {
            FindClosestEnemy();
            return;
        }

        // Only pathfind if the agent is active and we aren't mid-jump
        if (agent.isActiveAndEnabled && agent.isOnNavMesh && !isAttacking)
        {
            float distance = Vector3.Distance(transform.position, targetEnemy.transform.position);

            if (distance > attackRange)
            {
                agent.SetDestination(targetEnemy.transform.position);
            }
            else 
            {
                StartCoroutine(JumpAttackSequence());
            }
        }
    }

    private IEnumerator JumpAttackSequence()
    {
        isAttacking = true;
        agent.enabled = false; 

        if (rb != null)
        {
            rb.isKinematic = false; 
            Vector3 direction = (targetEnemy.transform.position - transform.position).normalized;
            
            // Add a jump arc (forward + slight upward lift)
            rb.AddForce((direction + Vector3.up * 0.5f) * jumpForce, ForceMode.Impulse);
        }

        // Wait for recovery time. If the dog hasn't hit an enemy by then, it "missed."
        yield return new WaitForSeconds(missRecoveryTime);

        if (isAttacking) 
        {
            isAttacking = false;
            ResetToNavMesh();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Only trigger damage if the dog is currently in its attack "jump" state
        if (isAttacking && collision.gameObject.CompareTag(enemyTag))
        {
            // Try to find the revamped enemy script
            var enemy = collision.gameObject.GetComponent<EnemyMovementControllerRevamped>();
            
            if (enemy != null)
            {
                enemy.Die();
                Debug.Log("Dog successfully hit the enemy!");
                
                isAttacking = false;
                ResetToNavMesh();
            }
        }
    }

    private void ResetToNavMesh()
    {
        if (rb != null) rb.isKinematic = true;

        // Use SamplePosition to find the nearest valid floor point 
        // This prevents the dog from getting stuck inside walls when dropped
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 2.0f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
            agent.enabled = true;
        }
        else
        {
            // If we can't find a NavMesh point, just turn the agent on and hope for the best
            agent.enabled = true;
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
}