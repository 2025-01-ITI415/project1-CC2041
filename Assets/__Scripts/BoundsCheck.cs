using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Keeps a GameObject within specified bounds.
/// Works for an orthographic Main Camera at [0,0,0].
/// </summary>
public class BoundsCheck : MonoBehaviour
{
    [Header("Set in Inspector")]
    public float radius = 1f;
    public bool keepOnScreen = true;

    public float penguinBoundX = 8f; // Set custom bounds for the penguin movement
    public float checkpointBoundX = 10f; // Set custom bounds for checkpoint spawning

    [Header("Set Dynamically")]
    public bool isOnScreen = true;
    public float camWidth;
    public float camHeight;
    [HideInInspector]
    public bool offRight, offLeft, offUp, offDown;
    [Header("Spawn Boundaries")]
    public float checkpointSpawnXBound = 8f; // Defines where checkpoints can spawn horizontally
    public float obstacleSpawnXBound = 8f;   // Defines where obstacles can spawn horizontally
    public float treeSpawnXOffset = 12f;     // Ensures trees spawn outside the playable area


    void Awake()
    {
        camHeight = Camera.main.orthographicSize;
        camWidth = camHeight * Camera.main.aspect;
    }

    void LateUpdate()
    {
        Vector3 pos = transform.position;
        isOnScreen = true;
        offRight = offLeft = offUp = offDown = false;

        // Adjusted bounds for the Penguin
        if (gameObject.CompareTag("Penguin"))
        {
            if (pos.x > penguinBoundX - radius)
            {
                pos.x = penguinBoundX - radius;
                offRight = true;
            }
            if (pos.x < -penguinBoundX + radius)
            {
                pos.x = -penguinBoundX + radius;
                offLeft = true;
            }
        }
        else // Default bounds for other objects (Checkpoints, Obstacles, etc.)
        {
            if (pos.x > camWidth - radius)
            {
                pos.x = camWidth - radius;
                offRight = true;
            }
            if (pos.x < -camWidth + radius)
            {
                pos.x = -camWidth + radius;
                offLeft = true;
            }
        }

        if (pos.y > camHeight - radius)
        {
            pos.y = camHeight - radius;
            offUp = true;
        }

        if (pos.y < -camHeight + radius)
        {
            pos.y = -camHeight + radius;
            offDown = true;
        }

        isOnScreen = !(offRight || offLeft || offUp || offDown);
        if (keepOnScreen && !isOnScreen)
        {
            transform.position = pos;
            isOnScreen = true;
            offRight = offLeft = offUp = offDown = false;
        }
    }

    // Adjusted method to get valid checkpoint spawn positions
    public Vector3 GetValidCheckpointPosition(float yOffset)
    {
        float xPos = Random.Range(-checkpointBoundX, checkpointBoundX);
        float yPos = camHeight + yOffset; // Ensure it spawns above the screen
        return new Vector3(xPos, yPos, 0);
    }

    // Draw bounds in the Scene view
    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(penguinBoundX * 2, camHeight * 2, 0.1f)); // Penguin bounds
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(checkpointBoundX * 2, camHeight * 2, 0.1f)); // Checkpoint bounds
    }
}
