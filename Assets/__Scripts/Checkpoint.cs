using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public float scrollSpeed = 5f; // Adjust based on game speed
    private float despawnHeight = 50f; // Y position where checkpoints get deleted
    private bool passed = false; // Prevents multiple triggers

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
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided with: " + other.gameObject.name); // Debugging

        if (!passed && other.CompareTag("Penguin")) // Ensure it's the penguin
        {
            passed = true; // Mark as passed
            
            Main.S.PlayerPassedCheckpoint(); // Notify Main
            Debug.Log("Checkpoint Passed!"); // Debugging check
        }
    }
}
