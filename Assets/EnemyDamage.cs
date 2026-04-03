using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public string muffin = "Muffin";

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag(muffin)) return;

        EnemyMovementController movement = GetComponent<EnemyMovementController>();
        if (movement != null)
        {
            movement.Die(); 
        }
    }
}