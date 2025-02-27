using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float scrollSpeed = 5f; // Speed at which obstacles move up
    private float despawnHeight = 50f; // Y position where obstacles get deleted

    void Update()
    {
        // Move the obstacle upwards
        transform.position += Vector3.up * scrollSpeed * Time.deltaTime;

        // Destroy when it moves past y = 50
        if (transform.position.y > despawnHeight)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Penguin")) // Make sure your Penguin has the "Player" tag
        {
            Main.S.PlayerHitObstacle(); // Calls function to reduce health
            Destroy(gameObject); // Remove obstacle on collision
        }
    }
}
