using System.Collections;
using UnityEngine;

public class EnemyMovementController : MonoBehaviour
{
    public Transform player;
    public float normalMoveSpeed = 2f;
    public float chasingMoveSpeed = 6f;
    public float turnSpeed = 4f;
    public float viewDistance = 10f;
    public float viewAngle = 60f;
    public float roamRadius = 3f;


    [Header("Obstacle Avoidance")]
    public float sensorDistance = 2f;
    public float sensorAngle = 30f; // spread
    public int sensorCount = 3;
    public LayerMask obstacleMask;

    [Header("Pause After Roam")]
    private Coroutine afterRoam;
    public float pauseAfterRoamChance = .2f;
    public float afterRoamPause = 2f;
    public float pauseSpinSpeed = 20f;

    private Rigidbody rb;
    private Vector3 roamTarget;

    void Start() {
        rb = GetComponent<Rigidbody>();
        GenerateRoamPoint();
    }

    public void FixedUpdate() {
        if (afterRoam != null) {
            // after roam state
            // look behind, but slower
            transform.Rotate(Vector3.up * pauseSpinSpeed * Time.fixedDeltaTime);
        } else if (CanSeePlayer()) {
            Debug.Log("CHASING PLAYER");
            MoveTowards(player.position, chasingMoveSpeed);
        } else {
            Roam();
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
        if(afterRoam != null) { return; } // just wait
        if (Vector3.Distance(transform.position, roamTarget) < 1f) {
            // hit roam point
            afterRoam = StartCoroutine(PauseBeforeRoamResume());
        }
        MoveTowards(roamTarget, normalMoveSpeed);
    }
    
    private IEnumerator PauseBeforeRoamResume() {
        if (Random.Range(0, 1f) <= pauseAfterRoamChance) {
            yield return new WaitForSeconds(afterRoamPause);
            GenerateRoamPoint();

            afterRoam = null;
        } else {
            GenerateRoamPoint();
            afterRoam = null;
        }
    }

    public void GenerateRoamPoint() {
        Vector3 randomDir = Random.insideUnitSphere * roamRadius;
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
