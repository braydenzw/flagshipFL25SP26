using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public string killItemTag = "Muffin";

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag(killItemTag)) return;

        EnemyMovementController movement = GetComponent<EnemyMovementController>();
        if (movement != null)
        {
            movement.Die(); 
        }
    }
}