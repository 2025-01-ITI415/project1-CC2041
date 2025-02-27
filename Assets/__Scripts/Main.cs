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

    [Header("Player Settings")]
    public int playerHealth = 3; // Starting health
    public int obstacleDamage = 1; // Damage per obstacle hit

    [Header("UI Settings")]
    public TextMeshProUGUI timerText; // UI Text to display elapsed time

    private BoundsCheck bndCheck;
    private int checkpointsPassed = 0;
    private float elapsedTime = 0f;
    private bool gameFinished = false;

    private void Awake()
    {
        S = this;
        bndCheck = GetComponent<BoundsCheck>();

        // Spawn all checkpoints at the start
        SpawnCheckpoints(totalCheckpoints);

        // Start spawning obstacles continuously
        InvokeRepeating("SpawnObstacle", 2f, 1f / obstacleSpawnRate);
    }

    private void Update()
    {
        if (!gameFinished)
        {
            elapsedTime += Time.deltaTime;
            timerText.text = "Time: " + elapsedTime.ToString("F2") + "s"; // Display elapsed time
        }
    }

    void SpawnCheckpoints(int count)
    {
        float spacing = 8f; // Space between checkpoint pairs
        for (int i = 0; i < count; i++)
        {
            float xPos = Random.Range(-bndCheck.camWidth + spacing, bndCheck.camWidth - spacing);

            // Left checkpoint
            Vector3 leftPos = new Vector3(xPos - spacing / 2, bndCheck.camHeight + spawnYOffset + (i * checkpointSpacing), 0);
            GameObject leftCheckpoint = Instantiate(prefabCheckpoint, leftPos, Quaternion.identity);

            // Right checkpoint
            Vector3 rightPos = new Vector3(xPos + spacing / 2, bndCheck.camHeight + spawnYOffset + (i * checkpointSpacing), 0);
            GameObject rightCheckpoint = Instantiate(prefabCheckpoint, rightPos, Quaternion.identity);
        }
    }

    void SpawnObstacle()
    {
        // Check if we've reached the max allowed obstacles
        if (activeObstacles.Count >= maxObstacles)
        {
            return;
        }

        GameObject go = Instantiate(prefabObstacle);

        Vector3 pos = new Vector3(
            Random.Range(-bndCheck.camWidth, bndCheck.camWidth),
            bndCheck.camHeight + spawnYOffset,
            0
        );
        go.transform.position = pos;

        activeObstacles.Add(go);
    }

    public void RemoveObstacle(GameObject obstacle)
    {
        activeObstacles.Remove(obstacle);
        Destroy(obstacle);
    }

    public void PlayerPassedCheckpoint()
    {
        checkpointsPassed++;
        if (checkpointsPassed >= totalCheckpoints)
        {
            GameOver();
        }
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
