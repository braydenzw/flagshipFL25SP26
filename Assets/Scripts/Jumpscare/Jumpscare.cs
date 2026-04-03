using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ImageFade : MonoBehaviour {
    public Image img;
    public float interval = 0.1f;

    public void Start() {
        img.color = new Color(1, 1, 1, 0);
    }

    public void OnCollisionEnter(Collision c) {
        if (c.gameObject.tag != "Player") { return; }
        img.color = new Color(1, 1, 1, 1);
        StartCoroutine(FadeImage());
    }

    public void OnTriggerEnter(Collider c) {
        if (c.gameObject.tag != "Player") { return; }
        img.color = new Color(1, 1, 1, 1);
        StartCoroutine(FadeImage());
    }

    IEnumerator FadeImage() {
        // loop over 1 second backwards
        for (float i = interval; i >= 0; i -= Time.deltaTime) {
            // set color with i as alpha
            img.color = new Color(1, 1, 1, i);
            yield return null;
        }
    }
}