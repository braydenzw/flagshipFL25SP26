using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealthController : MonoBehaviour
{
    Rigidbody rb;
    public float knockback = 2f;

    [Header("UI")]
    public RawImage health1;
    public RawImage health2;
    public RawImage health3;
    private int health = 3;

    public Color aliveColor;
    public Color deadColor;


    // Start is called before the first frame update
    void Start() {
        this.rb = GetComponent<Rigidbody>();
        health1.color = aliveColor;
        health2.color = aliveColor;
        health3.color = aliveColor;
        health = 3;
    }

    void OnCollisionEnter(Collision c) {
        if (c.gameObject.tag == "Enemy"){
            // remove health
            health--;
            switch (health) {
                case 2:
                    health3.color = deadColor;
                    break;
                case 1:
                    health2.color = deadColor;
                    break;
                default:
                    health1.color = deadColor;
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name); // reload
                    return;
            }

            // knockback player
            var dir = c.transform.position - rb.transform.position;
            dir.y = 0;
            rb.position = new Vector3(rb.position.x, rb.position.y + 1f, rb.position.z);
            Debug.Log(dir.normalized);
            rb.AddForce(dir.normalized * knockback * 10f);
        }   
    }
}
