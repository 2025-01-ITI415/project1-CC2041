using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // For UI timer

public class Main : MonoBehaviour
{
    static public Main S; // Singleton

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

    [Header("Tree Settings")]
    public GameObject prefabTree; // Tree prefab
    public float treeSpawnRate = 2f; // How often trees spawn
    public float treeSpawnXOffset = 12f; // Ensure trees spawn outside the playable bounds
    private List<GameObject> activeTrees = new List<GameObject>(); // Track active trees
    public float treeDespawnY = 50f; // Ensure trees only despawn when they reach a lower limit


    [Header("Player Settings")]
    public int playerHealth = 3; // Starting health
    public int obstacleDamage = 1; // Damage per obstacle hit

    [Header("UI Settings")]
    public TextMeshProUGUI timerText; // UI Text to display elapsed time
    public TextMeshProUGUI checkpointText; // UI to display remaining checkpoints

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


    private void Update()
    {
        if (!gameFinished)
        {
            elapsedTime += Time.deltaTime;
            timerText.text = "Time: " + elapsedTime.ToString("F2") + "s"; // Display elapsed time
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
                xPos = Random.Range(minX, maxX);
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

            GameObject checkpoint = Instantiate(prefabCheckpoint, new Vector3(xPos, yPos, 0), Quaternion.identity);
            Debug.Log($"Spawned Checkpoint at X: {xPos}, Y: {yPos}");

            usedPositions.Add(new Vector2(checkpoint.transform.position.x, checkpoint.transform.position.y));
        }
    }





    void SpawnObstacle()
    {
        // Check if we've reached the max allowed obstacles
        if (activeObstacles.Count >= maxObstacles)
        {
            return;
        }

        float xPos = Random.Range(-bndCheck.obstacleSpawnXBound, bndCheck.obstacleSpawnXBound);
        float yPos = bndCheck.camHeight + spawnYOffset;
        GameObject obstacle = Instantiate(prefabObstacle, new Vector3(xPos, yPos, 0), Quaternion.identity);
        activeObstacles.Add(obstacle);
    }

    public void RemoveObstacle(GameObject obstacle)
    {
        activeObstacles.Remove(obstacle);
        Destroy(obstacle);
    }

    public void PlayerPassedCheckpoint()
    {
        checkpointsPassed++;
        UpdateCheckpointUI();
        Debug.Log("Checkpoints Passed: " + checkpointsPassed + "/" + totalCheckpoints);
        if (checkpointsPassed >= totalCheckpoints)
        {
            GameOver();
        }
    }
    void SpawnTree()
    {
        float minTreeX = bndCheck.checkpointSpawnXBound + bndCheck.treeSpawnXOffset;
        float maxTreeX = -bndCheck.checkpointSpawnXBound - bndCheck.treeSpawnXOffset;

        float xPos = (Random.value > 0.5f) ? Random.Range(minTreeX, minTreeX + 5f) : Random.Range(maxTreeX - 5f, maxTreeX);
        float yPos = bndCheck.camHeight + spawnYOffset;
        
        GameObject tree = Instantiate(prefabTree, new Vector3(xPos, yPos, 0), Quaternion.identity);
        activeTrees.Add(tree);
    }



public void PlayerHitObstacle()
    {
        playerHealth -= obstacleDamage;
        if (playerHealth <= 0)
        {
            Debug.Log("Game Over! Out of health.");
            SceneManager.LoadScene("_Scene_0"); // Restart game
        }
    }

    void GameOver()
    {
        gameFinished = true;
        Debug.Log("Finished in " + elapsedTime.ToString("F2") + " seconds!");
        // Display finish screen here (optional)
    }
}
