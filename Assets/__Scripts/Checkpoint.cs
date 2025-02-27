using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public float scrollSpeed = 5f; // Adjust based on game speed
    private float despawnHeight = 50f; // Y position where checkpoints get deleted

    void Update()
    {
        // Move the checkpoint upwards
        transform.position += Vector3.up * scrollSpeed * Time.deltaTime;

        // Destroy when it moves past y = 50
        if (transform.position.y > despawnHeight)
        {
            Destroy(gameObject);
        }
    }
}
