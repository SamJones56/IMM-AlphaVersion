using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    // Declare initial state variables
    private int initialEnemiesKilled;
    private int initialCoinsCollected;
    private int initialRoundCounter;
    private int initialHealth;

    // Declare variables that will change
    private int enemiesKilled;
    public int coinsCollected;
    private int roundCounter;
    private int playerHealth;
    private bool isSkippedTutorial;

    // Initial health - To be modified by difficulty
    

    private float timeLeft;
    // Game Active
    public bool isGameActive = true;
    private bool hasRoundStarted = true;
    public bool playerHit = false;
    // Enemies drop coin
    private float coinChance;

    // UI elements
    public TextMeshProUGUI killedText;
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI playerHealthText;
    public TextMeshProUGUI tutorialText;

    // Game Objects
    public GameObject coinPrefab;
    // Spawn manager
    private SpawnManager spawnManager;
    // UI Controller
    private UIController uiController;
    // Shop
    public GameObject shopPrefab;
    // Ground
    public GameObject groundObject;
    // DataManager
    private DataManager dataManager;


    // Start is called before the first frame update
    void Start()
    {
        // Get the dataManager
        dataManager = FindObjectOfType<DataManager>();
        // Tutorial skip
        isSkippedTutorial = dataManager.GetSkippedTutorial();
        // Set the initial variables - We can edit these to change difficulty (Used for resetting)
        initialHealth = 3;
        initialEnemiesKilled = 0;
        initialRoundCounter = 0;
        initialCoinsCollected = 0;
        timeLeft = 20;
        coinChance = 0f;
        isGameActive = true;
        hasRoundStarted = true;
        playerHit = false;

        /* 
         * Logic for tutoral eg. first 3 rounds
         * Round 1 - Just running enemies
         * Round 2 - Just boom enemies
         * Round 3 - Just shooters
         * Round 4 - Tutorial end : all enemies
         * Round 5 - Rounds get harder from here
         */
         
        // Logic for first round DATA
        if(dataManager.roundCounter <= 1) 
        { 
            // Initial game state
            enemiesKilled = 0;
            coinsCollected = 0;
            playerHealth = initialHealth;
            roundCounter++;
            dataManager.SaveData(enemiesKilled, coinsCollected, roundCounter, playerHealth);
        }

        // Logic for later round DATA
        if (dataManager.roundCounter > 1) 
        {
            // Player health for when you skip the tutorial
            if (isSkippedTutorial) 
            {
                roundCounter = dataManager.roundCounter;
                playerHealth = initialHealth;
                dataManager.SetSkippedTutorial(false);
            }
            if (!isSkippedTutorial) 
            {
                playerHealth = dataManager.playerHealth;
                roundCounter = dataManager.roundCounter;
            }
            // Get data from dataManager
            enemiesKilled = dataManager.enemiesKilled;
            coinsCollected = dataManager.coinsCollected;
            // Update coins collected, enemies killed
            UpdateCoinCollected(0);
            UpdateEnemiesKilled(0);
            dataManager.SaveData(enemiesKilled, coinsCollected, roundCounter, playerHealth);
        }
        
        

        // Call SpawnManager
        spawnManager = FindObjectOfType<SpawnManager>();
        // Get the UIController
        uiController = FindObjectOfType<UIController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameActive)
        {
            timeLeft -= Time.deltaTime;
            Timer(timeLeft);
            RoundActive();
            PlayerHealth();
            
            // End the round
            if (timeLeft < 0)
            {
                RoundEnded();
            }
        }
    }
    // Time
    public void Timer(float timeLeft) 
    {
        timerText.SetText("Time: " + Mathf.Round(timeLeft));
    }
    // Update enemy killed UI
    public void UpdateEnemiesKilled(int killsToAdd) 
    {
        enemiesKilled += killsToAdd;
        killedText.text = "Enemies Killed: " + enemiesKilled;
    }

    // Coins collected
    public void UpdateCoinCollected(int coinsToAdd) 
    {
        coinsCollected += coinsToAdd;
        coinsText.text = "Coins: " + coinsCollected;
    }

    public void UpdateRoundText(int roundCounter)
    {
        roundText.text = "Round: " + dataManager.roundCounter;
    }

    public void UpdateHealthText() 
    {
        playerHealthText.text = "Health: " + dataManager.playerHealth;
    }

    public void CoinDrop(Vector3 enemyPosition) 
    {
        // Spawn coin 50% chance
        float coinDropChance = Random.Range(0.00f, 1.00f);
        if (coinChance <= coinDropChance)
        {
            // Soawn the coin with spin from CoinScript
            Instantiate(coinPrefab, enemyPosition, Quaternion.identity);
        }
    }

    public void RoundEnded() 
    {
        // Logic for ending the round
        roundCounter++;
        timeLeft = 10;
        isGameActive = false;
        spawnManager.SetRoundActive(false);
        spawnManager.CullEnemies();
        dataManager.SaveData(enemiesKilled, coinsCollected, roundCounter, playerHealth);
       
        ShopTime();
    }

    public void RoundActive() 
    {
        // Manage thr round being active
        if (isGameActive == true && hasRoundStarted == true)
        {
            UpdateRoundText(roundCounter);
            
            uiController.ShowUI(timerText);
            uiController.ShowUI(killedText);
            TutorialUI();
            spawnManager.SpawnRandomEnemy();
            hasRoundStarted = false;
        }
    }

    // Time for shop
    public void ShopTime()
    {
        //Load Shop Scene
        SceneManager.LoadScene(2);
    }

    public void NextRound() 
    {
        groundObject.SetActive(true);
        // Reset the time for the new round (e.g., 10 seconds)
        timeLeft = 30.0f;
        // Set the game as active for the new round
        isGameActive = true;
        // Reset the flag to allow spawning new enemies
        hasRoundStarted = true;
        spawnManager.SetRoundActive(true);
        // Call the method to spawn enemies for the new round
        RoundActive();
    }

    // Player health UI
    public void PlayerHealth() 
    {
        // Set the player health for ths skip & reset bool to false
        if (isSkippedTutorial) 
        {
            dataManager.playerHealth = initialHealth;
            playerHealth = dataManager.playerHealth;
            isSkippedTutorial = dataManager.GetSkippedTutorial();
        }
        UpdateHealthText();
        if (playerHit) {
            playerHealth -= 1;
            dataManager.playerHealth = playerHealth;
            UpdateHealthText();
            playerHit = false;
        }
        // You die
        if (playerHealth < 1)
        {
            // Reset variables
            playerHealth = initialHealth;
            coinsCollected = 0;

            // Reset the variables and load them into the data manager
            ResetVariables();
            dataManager.SaveData(enemiesKilled, coinsCollected, roundCounter, playerHealth);
                
            // Load death screen
            SceneManager.LoadScene(4);
        }
    }

    public void ResetVariables()
    {
        playerHealth = initialHealth;
        coinsCollected = initialCoinsCollected;
        roundCounter = initialRoundCounter;
        enemiesKilled = initialEnemiesKilled;
    }

    public void TutorialUI() 
    {
        if (dataManager.roundCounter == 1)
        {
            tutorialText.text = "WASD to Move, Left Click to Shoot";
        }
        if (dataManager.roundCounter > 1)
        {
            uiController.HideUI(tutorialText);
        }
    }
}
