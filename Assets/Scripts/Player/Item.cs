using UnityEngine;

// This ensures that any object with this script also has a Rigidbody component.
[RequireComponent(typeof(Rigidbody))]
public class Item : MonoBehaviour
{
    // This script is intentionally empty.
    // It's used only to identify which GameObjects are "Items" that can be picked up.
    // Later, we can add variables here, like item weight, ability to be displaced in time, etc.
}