using System.Collections;
using UnityEngine;

public class EnemyMovementController : MonoBehaviour
{
    public enum Phase {
        Roam, Search, Pause, Chase
    }

    public Transform player;
    public float normalMoveSpeed = 1.5f;
    public float chasingMoveSpeed = 6f;
    public float turnSpeed = 5f;
    public float viewDistance = 11f;
    public float viewAngle = 60f;
    public float roamRadius = 3f;


    [Header("Obstacle Avoidance")]
    public float sensorDistance = 2f;
    public float sensorAngle = 30f; // spread
    public int sensorCount = 3;
    public LayerMask obstacleMask;

    [Header("Pause After Roam")]
    public float pauseAfterRoamChance = .3f;
    public float afterRoamPause = 2f;

    [Header("Search")]
    public float searchAfterRoamChance = 0.2f;
    public float beforeSearchPause = 1f;
    public float searchTime = 2f;
    public float afterSearchTime = 1f;
    public float pauseSpinSpeed = 20f;

    [Header("Player Chase")]
    public float losePlayerChance = 0.3f;
    public float losePlayerInterval = 1f;
    private Coroutine losePlayerLoop = null;

    private Rigidbody rb;
    private Vector3 roamTarget;
    private Phase phase;

    void Start() {
        phase = Phase.Roam;
        rb = GetComponent<Rigidbody>();
        GenerateRoamPoint();
    }

    public void FixedUpdate() {
        switch (phase) {
            case Phase.Roam:
                Roam();
                if (CanSeePlayer()) {
                    phase = Phase.Chase;
                }
                break;
            case Phase.Search:
                transform.Rotate(Vector3.up * pauseSpinSpeed * Time.fixedDeltaTime);
                if (CanSeePlayer()) {
                    phase = Phase.Chase;
                }
                break;
            case Phase.Chase:
                if (losePlayerLoop == null) {
                    losePlayerLoop = StartCoroutine(LosePlayer());
                }
                MoveTowards(player.position, chasingMoveSpeed);
                break;
            case Phase.Pause:
                break;
        }
    }
    
    public IEnumerator LosePlayer() {
        while(true) {
            yield return new WaitForSeconds(losePlayerInterval);
            if (!CanSeePlayer() && Random.Range(0, 1f) <= losePlayerChance) {
                GenerateRoamPoint();
                StopCoroutine(losePlayerLoop);
                losePlayerLoop = null;
                phase = Phase.Roam;
                Debug.Log("Lost player...");
            }
        }
    }

    public bool CanSeePlayer() {
        Vector3 dirToPlayer = player.position - transform.position;
        float distance = dirToPlayer.magnitude;
        if (distance > viewDistance) { return false; }

        float angle = Vector3.Angle(transform.forward, dirToPlayer);
        if (angle > viewAngle / 2f) { return false; }

        if (Physics.Raycast(transform.position, dirToPlayer.normalized, out RaycastHit hit, viewDistance)) {
            return hit.transform == player;
        }
        return false;
    }

    public void MoveTowards(Vector3 target, float speed) {
        Vector3 dir = (target - transform.position).normalized;
        dir = ApplySensorAvoidance(dir);

        // Smoothly rotate toward desired direction
        Quaternion lookRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, turnSpeed * Time.fixedDeltaTime);

        rb.angularVelocity = Vector3.zero;
        rb.MovePosition(rb.position + transform.forward * speed * Time.fixedDeltaTime);
    }

    public void Roam() {
        if (Vector3.Distance(transform.position, roamTarget) < 1f) {
            // hit roam point
            StartCoroutine(PauseBeforeRoamResume());
        }
        MoveTowards(roamTarget, normalMoveSpeed);
    }
    
    private IEnumerator PauseBeforeRoamResume() {
        phase = Phase.Pause;
        if (Random.Range(0, 1f) <= pauseAfterRoamChance) {
            yield return new WaitForSeconds(afterRoamPause);
        }
        
        if (Random.Range(0, 1f) <= searchAfterRoamChance) {
            yield return new WaitForSeconds(beforeSearchPause);
            phase = Phase.Search;
            yield return new WaitForSeconds(searchTime);
            phase = Phase.Pause;
            yield return new WaitForSeconds(afterSearchTime);
        }
        
        GenerateRoamPoint();
        phase = Phase.Roam;
    }

    public void GenerateRoamPoint() {
        Vector3 randomDir = Random.insideUnitSphere * roamRadius * Random.Range(0.5f, 1.5f);
        randomDir.y = 0;
        roamTarget = transform.position + randomDir;
        Debug.Log("New roam point: " + roamTarget);
    }

    Vector3 ApplySensorAvoidance(Vector3 moveDir) {
        Vector3 avoidance = Vector3.zero;
        int hitCount = 0;
        if (
            Physics.Raycast(transform.position + Vector3.up * 0.2f,
            transform.forward, out RaycastHit hit,
            sensorDistance, obstacleMask)
        )
        {
            GenerateRoamPoint(); // if going to hit wall/get stuck, just go somewhere else
            avoidance += hit.normal;
            hitCount++;
        }

        // Side sensors
        for (int i = 1; i <= sensorCount; i++) {
            float angleOffset = sensorAngle * i / sensorCount;

            // Left
            Vector3 leftDir = Quaternion.AngleAxis(-angleOffset, Vector3.up) * transform.forward;
            if (Physics.Raycast(transform.position + Vector3.up * 0.2f, leftDir, out hit,
                sensorDistance / 2f, obstacleMask)
            ) {
                avoidance += hit.normal;
                hitCount++;
            }

            // Right
            Vector3 rightDir = Quaternion.AngleAxis(angleOffset, Vector3.up) * transform.forward;
            if (Physics.Raycast(transform.position + Vector3.up * 0.2f, rightDir, out hit,
                sensorDistance / 2f, obstacleMask)
            ) {
                avoidance += hit.normal;
                hitCount++;
            }
        }

        if (hitCount > 0) {
            avoidance.Normalize();
            moveDir = Vector3.Lerp(moveDir, avoidance, 0.8f);
        }
        return moveDir.normalized;
    }
}
