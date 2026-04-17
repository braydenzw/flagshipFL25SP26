using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

// author: Ethan Roberts
// based on: EnemyMovementController by Caleb
public class BraydenMovementControllerRevamped : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform target; // this is what the enemy is set to pursue when it is wandering
    private Transform player;
    [SerializeField] GameObject player_detector;
    private RaycastHit info;
    private enum states
    {
        searching,
        chasing,
        wandering
    }

    // use to define behavior (starts by searching)
    private states state = states.searching;

    private bool is_searching = false;
    private bool is_chasing = false;
    private bool is_wandering = false;
    private bool is_attacking = false;

    private Vector3 last_known_player_pos;

    // parameters
    [SerializeField] float chase_time = 5f;
    private float search_time = 5f;
    private float wander_time = 3f;
    private float wander_radius = 10f;
    private float attack_range = 1.8f;
    private float attack_cooldown = 2f;

    // audio
    [SerializeField] AudioClip foundPlayerClip;
    [SerializeField] AudioClip attackClip;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.Log("enemy does not have a NavMeshAgent attached");
        }

        // get player (must be tagged "Player")
        player = GameObject.FindWithTag("Player").transform;

        // enemy starts in wandering state
        state = states.wandering;
        StartCoroutine(ChasePlayer());
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case states.searching:
                if (!is_searching) StartCoroutine(Search());
                break;

            case states.wandering:
                if (!is_wandering) StartCoroutine(Wander());
                break;

            case states.chasing:
                if (!is_chasing) StartCoroutine(ChasePlayer());
                float distance_to_player = Vector3.Distance(player.position, transform.position);
                //Debug.Log("distance to player: " + distance_to_player);
                if (distance_to_player <= attack_range)
                {
                    if (!is_attacking) StartCoroutine(Attack());
                }
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

    // sets NavMesh target to player
    private void setTargetToPlayer()
    {
        if (player == null)
        {
            Debug.Log("enemy could not find player; check if player is tagged");
        }

        target = player;
    }

    // this is called by the player-detecting collider 
    // if player is found, disable player_detector (else enemy spams voicelines)
    public void foundPlayer()
    {
        Debug.Log("found the player");
        player_detector.SetActive(false);
        SoundManager.instance.PlaySound(foundPlayerClip, transform, 1f);
        is_searching = false;
        is_wandering = false;
        state = states.chasing;
    }

    // returns a Vector3 target in a random direction
    // (can be used as NavMesh target for enemy)
    private Vector3 getRandomTarget(float radius)
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

    // called after chase; 
    // the enemy goes to the player's last known position
    private IEnumerator Search()
    {
        is_searching = true;
        Debug.Log("searching the player's last known position");
        agent.SetDestination(last_known_player_pos);
        yield return new WaitForSeconds(search_time); 
        state = states.wandering;
        is_searching = false;
    }

    // continuously calculates random targets to walk to
    private IEnumerator Wander()
    {
        is_wandering = true;
        Debug.Log("wandering; recalculating target");
        Vector3 target = getRandomTarget(wander_radius);
        Vector3 direction_to_target = (target - transform.position).normalized;

        // if there is a wall or something blocking the way, end coroutine ear,y (need to recalculate target)
        if (Physics.Raycast(transform.position, direction_to_target, out info, 10f))
        {
            Debug.Log("enemy obstacle check hit: " + info.collider.gameObject);
            is_wandering = false;
            yield break;
        } 

        agent.SetDestination(target);
        yield return new WaitForSeconds(wander_time);
        is_wandering = false;
    }

    // chase the player, which will end after a delay chase_time
    // the enemy "loses sight" of the player and starts searching; this coroutine saves the last known position of the player
    // also enable player_detector (see foundPlayer())
    private IEnumerator ChasePlayer() {
        is_chasing = true;
        Debug.Log("enemy started chasing player");
        setTargetToPlayer();
        yield return new WaitForSeconds(chase_time);
        
        target = null;
        player_detector.SetActive(true);
        last_known_player_pos = player.transform.position;
        state = states.searching;
        is_chasing = false;
    }

    private IEnumerator Attack()
    {
        is_attacking = true;
        Debug.Log("attacking player");
        SoundManager.instance.PlaySound(attackClip, transform, 1f);
        player.gameObject.GetComponent<PlayerHealthController>().takeDamage(gameObject);
        yield return new WaitForSeconds(attack_cooldown);
        is_attacking = false;
    }
}
