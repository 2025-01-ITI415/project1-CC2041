using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // For UI timer

public class Main : MonoBehaviour
{
    static public Main S; // Singleton

    [Header("Set in Inspector")]
    public GameObject prefabCheckpoint; // Checkpoint prefab
    public GameObject prefabObstacle; // Obstacle prefab
    public int totalCheckpoints = 5; // Number of checkpoints required to win
    public float spawnYOffset = 10f; // Checkpoint spawn height above screen
    public float obstacleSpawnRate = 1f; // How often obstacles appear
    public Text timerText; // UI Text to display elapsed time

    [Header("Player Settings")]
    public int playerHealth = 3; // Starting health
    public int obstacleDamage = 1; // Damage per obstacle hit

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
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = new Vector3(
                Random.Range(-bndCheck.camWidth, bndCheck.camWidth),
                bndCheck.camHeight + spawnYOffset + (i * 5), // Spread them out
                0
            );

            GameObject go = Instantiate(prefabCheckpoint);
            go.transform.position = pos;
        }
    }

    void SpawnObstacle()
    {
        GameObject go = Instantiate(prefabObstacle);

        Vector3 pos = new Vector3(
            Random.Range(-bndCheck.camWidth, bndCheck.camWidth),
            bndCheck.camHeight + spawnYOffset,
            0
        );
        go.transform.position = pos;
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
