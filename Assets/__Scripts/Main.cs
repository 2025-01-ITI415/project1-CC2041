using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 

public class Main : MonoBehaviour
{
    static public Main S; // Singleton
    public float despawnHeight = 50; // Adjust as necessary

    [Header("Checkpoint Settings")]
    public GameObject prefabCheckpoint; // Checkpoint prefab
    public int totalCheckpoints = 5; // Number of checkpoints required to win
    public float spawnYOffset = 10f; // Checkpoint spawn height above screen
    public float checkpointSpacing = 10f; // Distance between checkpoints

    [Header("Obstacle Settings")]
    public GameObject prefabObstacle; // Obstacle prefab
    public int maxObstacles = 10; // Maximum number of obstacles allowed
    public float obstacleSpawnRate = 1f; // How often obstacles appear
    private List<GameObject> activeObstacles = new List<GameObject>(); // Track active obstacles
    public float rockYSpawnOffset = 10f; // Controls the Y spawn position of the rocks
    public float obstacleDespawnY = -10f;  // The Y position where obstacles should despawn
    public float obstacleSpacing = 10f; // The spacing between obstacles
    public float obstacleScrollSpeed = 5f; // The speed at which obstacles move upwards
    public float slowDuration = 2f; // Duration of the slowdown effect
    public float slowAmount = 0.05f; // Amount of speed reduction when the penguin hits the obstacle

    [Header("Tree Settings")]
    public GameObject prefabTree; // Tree prefab
    public float treeSpawnRate = 2f; // How often trees spawn
    public float treeSpawnXOffset = 12f; // Ensure trees spawn outside the playable bounds
    private List<GameObject> activeTrees = new List<GameObject>(); // Track active trees
    public float treeDespawnY = 50f; // Ensure trees only despawn when they reach a lower limit
    public float treeYSpawnOffset = 10f; // Controls the Y spawn position of the trees
    public float treeScrollSpeed = 5f; // The speed at which trees move upwards

    [Header("Player Settings")]
    public int playerHealth = 3; // Starting health
    public int obstacleDamage = 1; // Damage per obstacle hit
    public Penguin Penguin; // Reference to the other script


    [Header("UI Settings")]
    public TextMeshProUGUI checkpointText; // UI to display remaining checkpoints
    public GameObject winScreen; // Assign this in Unity UI
    public TextMeshProUGUI winTimeText; // Reference for the text showing the elapsed time on the win screen



    [Header("Movement Bounds")]
    public float penguinBoundX = 8f; // Set this to match the Penguin's movement bounds


    private BoundsCheck bndCheck;
    private int checkpointsPassed = 0;
    private float elapsedTime = 0f;
    private bool gameFinished = false;
    private List<float> usedYPositions = new List<float>(); // Track used y-positions

    private void Awake()
    {
        S = this;
        bndCheck = GetComponent<BoundsCheck>();

        // Spawn all checkpoints at the start
        SpawnCheckpoints(totalCheckpoints);

        // Start spawning obstacles continuously
        InvokeRepeating("SpawnObstacle", 2f, 1f / obstacleSpawnRate);
        InvokeRepeating("SpawnTree", 2f, 1f / treeSpawnRate);

        UpdateCheckpointUI();
    }

    void Update()
    {
        if (!gameFinished)
        {
            elapsedTime += Time.deltaTime;  // Increment time       

            MoveObjectsUp();
        }
    }

    void MoveObjectsUp()
    {
        // Loop through active obstacles and move them upwards
        foreach (GameObject obstacle in activeObstacles)
        {
            obstacle.transform.position += Vector3.up * obstacleScrollSpeed * Time.deltaTime;

            // Check if the obstacle goes past the despawn height
            if (obstacle.transform.position.y > despawnHeight)
            {
                RemoveObstacle(obstacle);
            }
        }

        // Loop through active trees and move them upwards
        foreach (GameObject tree in activeTrees)
        {
            tree.transform.position += Vector3.up * obstacleScrollSpeed * Time.deltaTime;

            // Check if the tree goes past the despawn height
            if (tree.transform.position.y > despawnHeight)
            {
                RemoveTree(tree);
            }
        }
    }


    void UpdateCheckpointUI()
    {
        int remainingCheckpoints = totalCheckpoints - checkpointsPassed;
        checkpointText.text = "Checkpoints Left: " + remainingCheckpoints;
    }

    void SpawnCheckpoints(int count)
    {
        float minX = -bndCheck.checkpointSpawnXBound;
        float maxX = bndCheck.checkpointSpawnXBound;

        float spacing = 8f;
        List<Vector2> usedPositions = new List<Vector2>();

        for (int i = 0; i < count; i++)
        {
            float xPos, yPos;
            bool positionValid;

            do
            {
                positionValid = true;
                xPos = UnityEngine.Random.Range(minX, maxX);
                yPos = bndCheck.camHeight + spawnYOffset + (i * checkpointSpacing);

                foreach (Vector2 pos in usedPositions)
                {
                    if (Vector2.Distance(new Vector2(xPos, yPos), pos) < checkpointSpacing)
                    {
                        positionValid = false;
                        break;
                    }
                }
            } while (!positionValid);

            usedPositions.Add(new Vector2(xPos, yPos));
            usedYPositions.Add(yPos);

            GameObject checkpoint = Instantiate(prefabCheckpoint, new Vector3(xPos, yPos, 0), Quaternion.identity);
        }
    }




    void SpawnObstacle()
    {
        if (activeObstacles.Count >= maxObstacles) return;

        float minX = -bndCheck.obstacleSpawnXBound;
        float maxX = bndCheck.obstacleSpawnXBound;

        // Define xPos and yPos for obstacles
        float xPos = UnityEngine.Random.Range(minX, maxX);

        // Define a yPos (ensure it spawns at an appropriate location)
        float yPos = UnityEngine.Random.Range(usedYPositions[0], usedYPositions[usedYPositions.Count - 1]) + rockYSpawnOffset;

        // Instantiate the obstacle (rock)
        GameObject obstacle = Instantiate(prefabObstacle, new Vector3(xPos, yPos, 0), Quaternion.identity);
        activeObstacles.Add(obstacle);
    }
    public void PlayerHitObstacle()
    {
        if (Penguin.S != null)
        {
            // Apply the slow effect using the correct field "speed"
            Penguin.S.applySlow(slowAmount, slowDuration); // Use the applySlow method in Penguin
        }

        // Decrease health
        playerHealth -= obstacleDamage;
        if (playerHealth <= 0)
        {
            SceneManager.LoadScene("Level 1"); // Restart the game
        }
    }




    private IEnumerator ResetPenguinSpeed()
    {
        // Wait for the duration of the slowdown effect
        yield return new WaitForSeconds(slowDuration);

        // Reset penguin speed to normal
        if (Penguin != null)
        {
            Penguin.penguinSpeed = 30f;  // Assuming 30f is the normal speed
        }
    }






    public void RemoveObstacle(GameObject obstacle)
    {
        activeObstacles.Remove(obstacle);
        Destroy(obstacle);
    }
    public void RemoveTree(GameObject tree)
    {
        activeTrees.Remove(tree);
        Destroy(tree);
    }

    public void PlayerPassedCheckpoint()
    {
        checkpointsPassed++;
        UpdateCheckpointUI();

        if (checkpointsPassed >= totalCheckpoints)
        {
            GameOver();
        }
    }

    void SpawnTree()
{
    float minTreeX = bndCheck.checkpointSpawnXBound + bndCheck.treeSpawnXOffset;
    float maxTreeX = -bndCheck.checkpointSpawnXBound - bndCheck.treeSpawnXOffset;

    float xPos = (UnityEngine.Random.value > 0.5f)
        ? UnityEngine.Random.Range(minTreeX, minTreeX + 5f)
        : UnityEngine.Random.Range(maxTreeX - 5f, maxTreeX);

    float yPos = bndCheck.camHeight + treeYSpawnOffset;

    // Ensure trees don't spawn too close to each other by checking the Y positions
    bool positionValid = false;
    for (int attempts = 0; attempts < 10; attempts++) // Limit to 10 attempts
    {
        positionValid = true;
        
        foreach (var tree in activeTrees)
        {
            if (Mathf.Abs(tree.transform.position.y - yPos) < checkpointSpacing) // Adjust distance here
            {
                positionValid = false;
                break;
            }
        }

        if (positionValid)
        {
            break; // If a valid position is found, stop trying
        }

        // If the position is invalid, try a different Y position
        yPos = UnityEngine.Random.Range(bndCheck.camHeight, bndCheck.camHeight + 10f);
    }

    if (positionValid)
    {
        // Instantiate the tree with the updated position
        GameObject tree = Instantiate(prefabTree, new Vector3(xPos, yPos, 0), Quaternion.identity);
        activeTrees.Add(tree);
    }
}


    

    




    

    void GameOver()
    {
        gameFinished = true;
        
        // Display finish screen 
        winScreen.SetActive(true);
        winScreen.GetComponentInChildren<TextMeshProUGUI>().text = "Finished in " + elapsedTime.ToString("F2") + " seconds!";
        Time.timeScale = 0;

    }
}
