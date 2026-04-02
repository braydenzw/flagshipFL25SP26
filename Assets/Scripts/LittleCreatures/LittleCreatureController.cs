using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LittleCreatureController : MonoBehaviour
{
    private Rigidbody rb;
    private CircleCollider2D col;

    void Start() {
        StartCoroutine("JumpChance");
    }

    public IEnumerator JumpChance() {
        yield return new WaitForSeconds(2f);
        
        if (Random.Range(0, 1) > 0.5f) {
            var direction = new Vector3(Random.Range(0, 3f), Random.Range(0, 3f), Random.Range(0, 3f));
            rb.AddForce(direction);
        }
        StartCoroutine("JumpChance");
    }
}
