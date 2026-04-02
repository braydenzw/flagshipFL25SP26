using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PictureChase : MonoBehaviour
{
    private Rigidbody rb;
    private Collider col;

    private Vector3 startPos;
    private Quaternion startAngle;

    public float speed = 1f;
    public float rotateSpeed = 10f;
    public float viewRange = 6f;
    public float timeToLose = 2f;
    public float timeToChase = 5f;
    public Transform player;
    public LayerMask playerLayer;

    private bool chasing;
    private float timer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        startPos = transform.position;
        startAngle = transform.rotation;
    }

    public void OnCollisionEnter(Collision c) {
        if (c.gameObject.tag != "Player") { return; }
        chasing = false;
        timer = 0f;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = startPos;
        transform.rotation = startAngle;
    }

    public void Chase(Vector3 direction)
    {
        Vector3 targetDirection = direction - transform.position;
        rb.velocity = targetDirection.normalized * speed;

        if ((direction - transform.position).sqrMagnitude < 4f){
            Debug.Log("force pos");
            chasing = false;
            timer = 0f;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            transform.position = direction;
            transform.rotation = startAngle;
        } else {
            Quaternion q = Quaternion.LookRotation(targetDirection.normalized, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, q, rotateSpeed);
        }
    }

    // Update is called once per frame
    void Update() {

        // if chasing and not in view lose after 2s
        // if not chasing and not in view, move back to position
        // if not chasing and in view, start chasing

        var playerInSight = Physics.CheckSphere(transform.position, viewRange, playerLayer);

        if (chasing) {
            Chase(player.transform.position);
            if (playerInSight) {
                timer = 0f;
            } else {
                timer += Time.deltaTime;
                if (timer > timeToLose) {
                    chasing = false;
                    timer = 0f;
                }
            }
        } else if (!playerInSight && (startPos - transform.position).sqrMagnitude > 3f) {
            Chase(startPos);
        } else if (playerInSight) {
            timer += Time.deltaTime;
            if (timer > timeToChase) {
                chasing = true;
            } else {
                transform.Rotate(new Vector3(Random.Range(-timer, timer), 0, 0), Space.World);
            }
        } else {
            timer = 0f;
        }
    }
}
