using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    // DataManager
    private DataManager dataManager;

    // Decalre object for enemy prefabs
    public GameObject[] enemyPrefabs;
    // Boundary
    private float minX = -20.0f;
    private float maxX = 20.0f;
    private float minZ = -20.0f;
    private float maxZ = 20.0f;
    // y position
    private float yPos = 1;

    // Delay for Spawn
    private float startDelay = 2.0f;
    private float spawnInterval = 1.0f;

    // Round counter
    bool isGameActive = true;

    // List of active enemies
    private List<GameObject> activeEnemies = new List<GameObject>();
    // List of all active bullets
    public List<GameObject> activeBullets = new List<GameObject>();
    // Shop Prefabs
    public GameObject[] shopPrefabs;
    // Enemy types for spawning
    public GameObject runnerEnemy;
    public GameObject boomEnemy;
    public GameObject shooterEnemy;
    public GameObject[] allEnemyPrefabs;
    // Round counter
    private int roundCounter;
    

    void Start() 
    {
        // Get the dataManager
        dataManager = FindObjectOfType<DataManager>();

        // Initialise round counter
        roundCounter = dataManager.roundCounter;

        UpdateEnemyPrefabs(roundCounter);
    }

    // Update the prefabs
    void UpdateEnemyPrefabs(int round)
    {
        if (round == 1)
        {
            enemyPrefabs = new GameObject[] { runnerEnemy };
        }
        else if (round == 2)
        {
            enemyPrefabs = new GameObject[] { boomEnemy };
        }
        else if (round == 3)
        {
            enemyPrefabs = new GameObject[] { shooterEnemy };
        }
        else
        {
            enemyPrefabs = allEnemyPrefabs;
        }
    }


    // Spawn random ball at random x and y position in play area
    public void SpawnRandomEnemy()
    {
        // Spawn enemy
        if (isGameActive)
        {
            // Random position
            float randomX = Random.Range(minX, maxX);
            float randomZ = Random.Range(minZ, maxZ);
            // Generate random enemy index and random spawn position
            Vector3 spawnPos = new Vector3(randomX, yPos, randomZ);
            // instantiate enemy at random spawn location
            // Random enemy number
            int enemyArrayPos = Random.Range(0, enemyPrefabs.Length);
            GameObject enemyInstance = Instantiate(enemyPrefabs[enemyArrayPos], spawnPos, enemyPrefabs[enemyArrayPos].transform.rotation);
            // spawn ball at random times
            spawnInterval = Random.Range(startDelay, 4f);

            // Schedule the next enemy spawn
            spawnInterval = Random.Range(startDelay, 4f);
            Invoke("SpawnRandomEnemy", spawnInterval);

            // Add the enemies to the list as they spawn
            activeEnemies.Add(enemyInstance);
        }
        
    }



    // Set round over to stop enemies spawning
    public void SetRoundActive(bool isGameActive)
    {
        this.isGameActive = isGameActive;
    }




    // Cull active enemies
    public void CullEnemies() 
    {
        // Loop through the list of active enemies and destroy them
        foreach (var enemy in activeEnemies)
        {
            Destroy(enemy);
        }

        foreach (var bullet in activeBullets) 
        {
            Destroy(bullet);
        }

        // Clear the list of active enemies
        activeEnemies.Clear();
        activeBullets.Clear();
    }
}

