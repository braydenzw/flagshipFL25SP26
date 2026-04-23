using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovementControllerRevamped : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform target; 
    private Transform player;
    [SerializeField] GameObject player_detector;
    
    private enum States { Searching, Chasing, Wandering, Dead }
    [SerializeField] private States currentState = States.Wandering;

    // Parameters
    [SerializeField] float chase_time = 5f;
    [SerializeField] float search_time = 5f;
    [SerializeField] float wander_time = 3f;
    [SerializeField] float wander_radius = 10f;
    [SerializeField] float attack_range = 1.8f;
    [SerializeField] float attack_cooldown = 2f;

    private bool isCoroutineRunning = false;
    private bool isAttacking = false;
    private Vector3 last_known_player_pos;

    [Header("Audio")]
    [SerializeField] AudioClip foundPlayerClip;
    [SerializeField] AudioClip attackClip;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player").transform;
        currentState = States.Wandering;
    }

    void Update()
    {
        if (currentState == States.Dead) return;

        // Only start a new state routine if one isn't already running
        if (!isCoroutineRunning)
        {
            switch (currentState)
            {
                case States.Wandering:
                    StartCoroutine(WanderRoutine());
                    break;
                case States.Searching:
                    StartCoroutine(SearchRoutine());
                    break;
                case States.Chasing:
                    StartCoroutine(ChaseRoutine());
                    break;
            }
        }

        // Logic that needs to happen every frame during Chasing
        if (currentState == States.Chasing)
        {
            agent.SetDestination(player.position);
            
            float dist = Vector3.Distance(player.position, transform.position);
            if (dist <= attack_range && !isAttacking)
            {
                StartCoroutine(AttackRoutine());
            }
        }
    }

    //for doggie
    public void Die()
    {
        currentState = States.Dead;
        StopAllCoroutines(); 
        agent.isStopped = true;
        agent.enabled = false;
        
        Debug.Log("Enemy has been defeated by the dog!");
        //can add death animation or idk
        Destroy(gameObject, 0.5f); 
    }

    public void foundPlayer()
    {
        if (currentState == States.Chasing) return;
        
        Debug.Log("found the player");
        player_detector.SetActive(false);
        if(SoundManager.instance) SoundManager.instance.PlaySound(foundPlayerClip, transform, 1f);
        
        StopAllCoroutines(); // Interrupt wandering/searching
        isCoroutineRunning = false;
        currentState = States.Chasing;
    }

    private IEnumerator WanderRoutine()
    {
        isCoroutineRunning = true;
        Vector3 randomTarget = getRandomTarget(wander_radius);
        agent.SetDestination(randomTarget);
        
        yield return new WaitForSeconds(wander_time);
        isCoroutineRunning = false;
    }

    private IEnumerator ChaseRoutine()
    {
        isCoroutineRunning = true;
        yield return new WaitForSeconds(chase_time);
        
        // After chase_time, if player isn't "found" again, go to search
        last_known_player_pos = player.position;
        player_detector.SetActive(true);
        currentState = States.Searching;
        isCoroutineRunning = false;
    }

    private IEnumerator SearchRoutine()
    {
        isCoroutineRunning = true;
        agent.SetDestination(last_known_player_pos);
        
        yield return new WaitForSeconds(search_time);
        currentState = States.Wandering;
        isCoroutineRunning = false;
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        if(SoundManager.instance) SoundManager.instance.PlaySound(attackClip, transform, 1f);
        
        var health = player.GetComponent<PlayerHealthController>();
        if (health != null) health.takeDamage(gameObject);
        
        yield return new WaitForSeconds(attack_cooldown);
        isAttacking = false;
    }

    private Vector3 getRandomTarget(float radius)
    {
        Vector3 dir = Random.insideUnitSphere * radius;
        dir += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(dir, out hit, radius, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return transform.position;
    }
}