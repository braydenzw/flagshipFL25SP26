using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementController : MonoBehaviour
{
    public enum Phase {
        Roam, Search, Pause, Chase, TimeTravel
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
    public float losePlayerInterval = 0.8f;
    private Coroutine losePlayerLoop = null;

    [Header("Time Travel")]
    public float timeTravelChance = 0.1f;
    public int minSteps = 2;
    public int maxSteps = 5;
    public float timeTravelSpinVelocity = 4000f;
    public float timeTravelTime = 2.5f; // time it takes to do it
    private Queue<Vector3> pastPositions;
    private Queue<Vector3> futurePositions;

    private Coroutine temp;
    private Rigidbody rb;
    private Vector3 roamTarget;
    private Phase phase;

    void Start() {
        phase = Phase.Roam;
        rb = GetComponent<Rigidbody>();
        pastPositions = new Queue<Vector3>();
        futurePositions = new Queue<Vector3>();
        GenerateRoamPoint();
    }

    public void FixedUpdate() {
        // if (Input.GetKeyDown(KeyCode.T))
        // {
        //     StopCoroutine("DoTimeTravel");
        //     StartCoroutine("DoTimeTravel");
        // }
        switch (phase) {
            case Phase.Roam:
                Roam();
                if (CanSeePlayer()) {
                    phase = Phase.Chase;
                }
                break;
            case Phase.Search:
                rb.MoveRotation(
                    rb.rotation * Quaternion.Euler(0f, pauseSpinSpeed * Time.fixedDeltaTime, 0f)
                );
                if (CanSeePlayer()) {
                    phase = Phase.Chase;
                }
                break;
            case Phase.Chase:
                if (temp != null) {
                    StopCoroutine(temp);
                    temp = null;
                }
                if (losePlayerLoop == null) {
                    losePlayerLoop = StartCoroutine(LosePlayer());
                }
                MoveTowards(player.position, chasingMoveSpeed);
                break;
            case Phase.TimeTravel:
                if (temp != null) {
                    StopCoroutine(temp);
                    temp = null;
                }
                rb.MoveRotation(
                    rb.rotation * Quaternion.Euler(0f, timeTravelSpinVelocity * Time.fixedDeltaTime, 0f)
                );
                break;
            case Phase.Pause:
                break;
        }
    }
    
    public IEnumerator LosePlayer() {
        while(true) {
            yield return new WaitForSeconds(losePlayerInterval);
            if (!CanSeePlayer() && Random.Range(0, 1f) <= losePlayerChance) {
                Debug.Log("Lost player...");
                StopCoroutine(losePlayerLoop);
                losePlayerLoop = null;
                pastPositions.Enqueue(transform.position); // save this position as meaningful
                
                GenerateRoamPoint();
                phase = Phase.Roam;
            }
        }
    }

    public bool CanSeePlayer() {
        Vector3 dirToPlayer = player.position - transform.position;
        float distance = dirToPlayer.magnitude;
        if (distance > viewDistance) { return false; }

        float angle = Vector3.Angle(rb.transform.forward, dirToPlayer);
        if (angle > viewAngle / 2f) { return false; }

        if (Physics.Raycast(transform.position, dirToPlayer.normalized, out RaycastHit hit, viewDistance)) {
            return hit.transform == player;
        }
        return false;
    }

    public void MoveTowards(Vector3 target, float speed) {
        Vector3 dir = (target - rb.position).normalized;
        dir = ApplySensorAvoidance(dir);

        // flatten to only move around horizontal (dont break freeze rotation)
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.001f) {
            dir.Normalize();
        }

        // Smoothly rotate toward desired direction
        Quaternion lookRot = Quaternion.LookRotation(dir);
        rb.MoveRotation(
            Quaternion.Slerp(rb.rotation, lookRot, turnSpeed * Time.fixedDeltaTime)
        );

        var move = Vector3.MoveTowards(rb.position, rb.position + rb.transform.forward, speed * Time.fixedDeltaTime);
        rb.MovePosition(move);
        // rb.MovePosition(rb.position + speed * Time.fixedDeltaTime * rb.transform.forward);
    }

    public void Roam() {
        if (Vector3.Distance(rb.position, roamTarget) < 1f) {
            // hit roam point
            temp = StartCoroutine(PauseBeforeRoamResume());
        }
        MoveTowards(roamTarget, normalMoveSpeed);
    }
    
    private IEnumerator DoTimeTravel() {
        Debug.Log("doing time travel...");
        phase = Phase.TimeTravel;
        yield return new WaitForSeconds(timeTravelTime);
        Vector2 pos = transform.position;

        if (Random.Range(0, 1f) < 0.5f) {
            for (int i = 0; i < Mathf.Max(pastPositions.Count - 1, Random.Range(minSteps, maxSteps)); i++) {
                if (pastPositions.Count > 0) {
                    Debug.Log("Pop past pos");
                    pos = pastPositions.Dequeue();
                    futurePositions.Enqueue(pos); // going backwards, so this is now a futurePos
                }
            }
        } else {
            for (int i = 0; i < Mathf.Max(futurePositions.Count -1, Random.Range(minSteps, maxSteps)); i++) {
                if (futurePositions.Count > 0) {
                    Debug.Log("Pop future pos");
                    pos = futurePositions.Dequeue();
                    pastPositions.Enqueue(pos); // going forwards, so this is now a pastPos
                }
            }
        }
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.position = pos;

        pastPositions.Enqueue(rb.position);
        GenerateRoamPoint();
        phase = Phase.Roam;
    }
    
    public IEnumerator PauseBeforeRoamResume() {
        phase = Phase.Pause;
        pastPositions.Enqueue(transform.position);
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
        
        // if failed search, consider time traveling backwards
        if (Random.Range(0, 1f) <= timeTravelChance) {
            StartCoroutine(DoTimeTravel());
        } else {
            GenerateRoamPoint();
            if (futurePositions.Count > 0) {
                futurePositions.Dequeue(); // creating new future...!
            }
            phase = Phase.Roam;
        }
    }

    public void GenerateRoamPoint() {
        Vector3 randomDir = Random.insideUnitSphere * roamRadius * Random.Range(0.5f, 1.5f);
        randomDir.y = 0;
        roamTarget = transform.position + randomDir;
    }
    
    

    Vector3 ApplySensorAvoidance(Vector3 moveDir) {
        Vector3 avoidance = Vector3.zero;
        int hitCount = 0;
        if (
            Physics.Raycast(transform.position + Vector3.up * 0.2f,
            rb.transform.forward, out RaycastHit hit,
            sensorDistance, obstacleMask)
        )
        {
            Debug.Log("GENERATING NEW ROAM FOR SENSOR AVOID");
            GenerateRoamPoint(); // if going to hit wall/get stuck, just go somewhere else
            avoidance += hit.normal;
            hitCount++;
        }

        // Side sensors
        for (int i = 1; i <= sensorCount; i++) {
            float angleOffset = sensorAngle * i / sensorCount;

            // Left
            Vector3 leftDir = Quaternion.AngleAxis(-angleOffset, Vector3.up) * rb.transform.forward;
            if (Physics.Raycast(transform.position + Vector3.up * 0.2f, leftDir, out hit,
                sensorDistance / 2f, obstacleMask)
            ) {
                avoidance += hit.normal;
                hitCount++;
            }

            // Right
            Vector3 rightDir = Quaternion.AngleAxis(angleOffset, Vector3.up) * rb.transform.forward;
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
