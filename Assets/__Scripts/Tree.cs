using UnityEngine;

public class Tree : MonoBehaviour
{
    public float scrollSpeed = 5f; // Adjust tree scroll speed
    private float despawnY = -50f; // Y position where trees despawn

    void Update()
    {
        transform.position += Vector3.down * scrollSpeed * Time.deltaTime;

        // Remove the tree when it moves off-screen
        if (transform.position.y < despawnY)
        {
            Destroy(gameObject);
        }
    }
}
