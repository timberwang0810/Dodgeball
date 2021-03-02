﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class GameManager : MonoBehaviour
{
    // Game States
    public enum GameState { menu, getReady, playing, paused, oops, gameOver };
    public GameState gameState;
    public static GameManager S;

    public GameObject ballPrefab;
    private GameObject currentPlayer;

    // Opening Cinematic image
    public GameObject cinematic;
    private bool seenCinematic = false;

    // UI Variables
    [Header("Basic UI Variables")]
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI endText;
    public GameObject pausePanel;
    public GameObject gameOverPanel;
    public GameObject controlPanel;

    [Header("Power Bar")]
    public Image powerUpBarFill;
    public GameObject powerUpBarCanvas;
    public Image powerUpBarFire;
    [Range(0,1)]
    public float parryPowerUp;
    [Range(0, 1)]
    public float hitPowerUp;
    public float buffDuration;
    public float timeBetweenBallSpawn;
    public float powerUpDecrementRate;

    [Header("Progress Bar")]
    public Image progressBarFill;
    public GameObject progressBarCanvas;

    [Header("Game Variables")]
    public int maxLevel;
    public int lives;
    public int getReadyTime;
    public int maxEnemiesOnCourt;
    public float timeBetweenEnemySpawn;
    
    private int numEnemies;
    private int score;
    private bool powerFilled;
    private float powerUpTimer;
    private Vector3 spawnPos;
    private Dictionary<GameObject, int> maxEnemies = new Dictionary<GameObject, int>();
    private Dictionary<string, int> currEnemies = new Dictionary<string, int>();
    private int numEnemiesOnCourt;
    private int numEnemiesToSpawn;
    private int totalNumEnemies;

    [Header("Audience")]
    public GameObject audience;
    public float maxHype;
    public float enemyPoints;
    public float hypeCooldown;
    private float hypeTimer = 0;
    private float hype = 0;

    public Texture2D cursorTexture;
    private Vector2 cursorOffset;

    // Struct to organize enemy spawning
    [System.Serializable]
    public struct EnemyCountPair
    {
        public GameObject enemyPrefab;
        public int enemyCount;
    }

    private void Awake()
    {
        // Singleton Definition
        if (GameManager.S)
        {
            // singleton exists, delete this object
            Destroy(this.gameObject);
        }
        else
        {
            S = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
        Cursor.visible = true;
        pausePanel.SetActive(false);
        controlPanel.SetActive(false);
        scoreText.text = "Score: " + 0;
        Time.timeScale = 1;
        cursorOffset = new Vector2(cursorTexture.width / 2, cursorTexture.height / 2);
        Cursor.SetCursor(cursorTexture, cursorOffset, CursorMode.Auto);
    }

    private void Update()
    {
        if (!powerFilled && currentPlayer != null) // edge case where you get hit then power up
        {
            SoundManager.S.StopPoweredUpSound();
            currentPlayer.GetComponent<ParticleSystem>().Stop();
        }


        if (gameState == GameState.playing)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                OnPause();
            powerUpTimer += Time.deltaTime;
            if (!powerFilled && powerUpTimer >= 1.0f) powerUpBarFill.fillAmount = Mathf.Clamp(powerUpBarFill.fillAmount - powerUpDecrementRate, 0, 1);

            hypeTimer += Time.deltaTime;

            if (hype >= maxHype)
            {
                if (hypeTimer >= hypeCooldown)
                {
                    audience.GetComponent<Audience>().cheer();
                } else
                {
                    //do nothing
                }
                hype = 0;
                hypeTimer = 0;
            }
            hype -= Time.deltaTime;
            if (hype < 0) hype = 0;
        }

        else if (gameState == GameState.paused) {
            if (Input.GetKeyDown(KeyCode.Escape)) OnUnpause();
        }


    }

    // Called by LevelManager
    public void StartNewGameWrapper()
    {
        if (seenCinematic)
        {
            StartNewGame();
        } else
        {
            StartCoroutine(showCinematic());
            seenCinematic = true;
        }
    }

    // Start a new round
    public void StartNewGame()
    {

        currentPlayer = GameObject.FindGameObjectWithTag("Player");
        spawnPos = currentPlayer.transform.position;
        gameState = GameState.getReady;
        numEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
        powerUpBarFill.fillAmount = 0;
        progressBarFill.fillAmount = 0;

        foreach (EnemyCountPair p in LevelManager.S.maxEnemies)
        {
            maxEnemies[p.enemyPrefab] = p.enemyCount;
            numEnemiesToSpawn += p.enemyCount;
            currEnemies[p.enemyPrefab.name] = 0;
        }
        totalNumEnemies = numEnemiesToSpawn;
        ResetLevel();
    }

    // Display the opening cinematic
    private IEnumerator showCinematic()
    {
        cinematic.SetActive(true);
        scoreText.enabled = false;
        powerUpBarCanvas.SetActive(false);
        progressBarCanvas.SetActive(false);
        statusText.enabled = false;
        yield return new WaitForSeconds(3);
        cinematic.SetActive(false);
        scoreText.enabled = true;
        powerUpBarCanvas.SetActive(true);
        progressBarCanvas.SetActive(true);
        statusText.enabled = true;
        StartNewGame();
    }

    // Reset a round (called after new round or after player death)
    private void ResetLevel()
    {
        hype = 0;
        hypeTimer = 0;
        powerUpTimer = 0;
        powerUpBarFill.fillAmount /= 2;
        powerUpBarFire.enabled = false;
        powerFilled = false;

        StartCoroutine(GetReady());
    }

    // Get Ready sequence
    private IEnumerator GetReady()
    {
        statusText.enabled = true;
        statusText.text = "Get Ready!";
        for (int i = 0; i < getReadyTime; i++)
        {
            statusText.text = (getReadyTime - i).ToString();
            yield return new WaitForSeconds(1);
        }
        StartRound();
        SoundManager.S.StartWhistle();
        statusText.text = "Go!";
        yield return new WaitForSeconds(1);
        statusText.enabled = false;
        
    }

    // Start a round (start spawning enemy)
    private void StartRound()
    {
        gameState = GameState.playing;
        StartCoroutine(SpawnEnemies());
    }

    // Spawning enemy sequence
    private IEnumerator SpawnEnemies()
    {
        while (gameState == GameState.playing)
        {
            if (numEnemiesOnCourt < maxEnemiesOnCourt)
            {
                SpawnOneEnemy();
            }
            yield return new WaitForSeconds(timeBetweenEnemySpawn);
        }   
    }

    // Spawn a random enemy type dictated by the level description in LevelManager
    private void SpawnOneEnemy()
    {
        if (numEnemiesToSpawn <= 0) return;

        // Choose a random enemy type
        GameObject enemyPrefab = maxEnemies.ElementAt(Random.Range(0, maxEnemies.Count())).Key;
        while (currEnemies[enemyPrefab.name] >= maxEnemies[enemyPrefab])
        {
            enemyPrefab = maxEnemies.ElementAt(Random.Range(0, maxEnemies.Count())).Key;
        }
        // Instantiate enemy at spawn location
        Instantiate(enemyPrefab, new Vector3(LevelManager.S.enemySpawner.transform.position.x, Random.Range(-13, 13), 0), Quaternion.identity);
        currEnemies[enemyPrefab.name] += 1;
        numEnemies++;
        numEnemiesOnCourt++;
        numEnemiesToSpawn--;
    }

    // Called when a round is won 
    private void RoundWon()
    {
        hype = 0;
        hypeTimer = 0;
        audience.GetComponent<Audience>().resetCheer();
        gameState = GameState.oops;
        ResetPowerUp(true);

        // Go to end if all levels are completed, or go to the next level
        if (LevelManager.S.currLevel >= maxLevel)
        {
            GameWon();
        }
        else
        {
            LevelManager.S.GoToNextLevel();
        }
    }

    // Called when the game is won
    private void GameWon()
    {
        SoundManager.S.GameWinSound();
        statusText.enabled = false;
        gameState = GameState.gameOver;
        endText.text = "Game Won";
        gameOverPanel.SetActive(true);
    }

    // Called when the player has died
    public void playerDied()
    {
        if (gameState == GameState.oops) return;
        gameState = GameState.oops;
        StopAllCoroutines();
        ResetPowerUp(false);
        currentPlayer.GetComponent<CapsuleCollider2D>().enabled = false;
        lives -= 1;
        hype = 0;
        hypeTimer = 0;
        audience.GetComponent<Audience>().resetCheer();
        SoundManager.S.StopPoweredUpSound();

        // Go to lose state if all lives are lost, or restart the level
        if (lives > 0)
        {
            StartCoroutine(betweenRoundsLost());
            SoundManager.S.PlayerHitSound();
        }
        else
        {
            StartCoroutine(gameOver());
            SoundManager.S.GameLoseSound();
            gameState = GameState.gameOver;
            
        }
    }

    // Sequence after a round has been won
    private IEnumerator betweenRoundsWon()
    {
        statusText.text = LevelManager.S.currLevelName + " Complete!";
        statusText.enabled = true;
        currentPlayer.GetComponent<CapsuleCollider2D>().enabled = false;
        yield return new WaitForSeconds(3);
        currentPlayer.GetComponent<CapsuleCollider2D>().enabled = true;
        RoundWon();
    }

    // Sequence after the player has lost all three lives
    private IEnumerator gameOver()
    {
        SoundManager.S.StopPoweredUpSound();
        currentPlayer.GetComponent<ParticleSystem>().Stop();
        currentPlayer.GetComponent<CapsuleCollider2D>().enabled = false;
        yield return new WaitForSeconds(1);
        currentPlayer.GetComponent<Renderer>().enabled = false;
        gameOverPanel.SetActive(true);
    }


    // Sequence after a round has been lost
    private IEnumerator betweenRoundsLost()
    {
        
        statusText.text = "Lives Left:" + lives;
        statusText.enabled = true;
        yield return new WaitForSeconds(1);
        currentPlayer.GetComponent<Renderer>().enabled = false;
        yield return new WaitForSeconds(1);
        currentPlayer.GetComponent<Renderer>().enabled = true;
        currentPlayer.GetComponent<CapsuleCollider2D>().enabled = true;
        currentPlayer.transform.position = spawnPos;
        ResetLevel();
    }

    // Called when an enemy has been destroyed
    public void OnEnemyDestroyed()
    {
        numEnemies--;
        numEnemiesOnCourt--;
        hype += enemyPoints;

        // Update progress and power-ups
        IncreasePower(hitPowerUp);
        IncreaseProgress();

        // Round is won when all enemies are destroyed
        if (numEnemies <= 0 && numEnemiesToSpawn <= 0)
        {
            StartCoroutine(betweenRoundsWon());
        }
    }

    // Called when the player successfully perform a parry
    public void OnSuccessfulParry()
    {
        IncreasePower(parryPowerUp);
    }

    // Called when the score is updated
    public void OnScoreAdded(int addedScore)
    {
        score += addedScore;
        scoreText.text = "Score: " + score;
    }

    // Called when the player pauses the game
    private void OnPause()
    {
        pausePanel.SetActive(true);
        gameState = GameState.paused;
        Time.timeScale = 0;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    // Called when the player unpauses the game
    private void OnUnpause()
    {
        pausePanel.SetActive(false);
        controlPanel.SetActive(false);
        gameState = GameState.playing;
        Time.timeScale = 1;
        Cursor.SetCursor(cursorTexture, cursorOffset, CursorMode.Auto);
    }

    // Accessor to powerFilled boolean
    public bool isPowerFilled()
    {
        return powerFilled;
    }

    // Helper to increment the power bar
    private void IncreasePower(float increment)
    {
        powerUpBarFill.fillAmount = Mathf.Clamp(powerUpBarFill.fillAmount + increment, 0, 1);
        if (powerUpBarFill.fillAmount >= 1)
        {
            if (!powerFilled) SoundManager.S.PoweredUpSound();
            StartCoroutine(Buffed());
        }
    }
    
    // Helper to increment the progress bar
    private void IncreaseProgress()
    {
        progressBarFill.fillAmount = (totalNumEnemies - numEnemiesToSpawn) / (float)totalNumEnemies;
    }

    // Sequence during player buff (maxed out power bar)
    private IEnumerator Buffed()
    {
        powerFilled = true;
        powerUpBarFire.enabled = true;
        currentPlayer.GetComponent<ParticleSystem>().Play();
        currentPlayer.GetComponent<Animator>().SetBool("holding", true);
        yield return new WaitForSeconds(buffDuration);
        ResetPowerUp(true);
    }

    // Helper to reset the power bar
    // Clear powerbar if reset is true
    private void ResetPowerUp(bool reset)
    {
        currentPlayer.GetComponent<Animator>().SetBool("holding", false);
        currentPlayer.GetComponent<ParticleSystem>().Stop();
        powerFilled = false;
        powerUpBarFire.enabled = false;
        if (reset) powerUpBarFill.fillAmount = 0;
        powerUpTimer = 0;
    }

    public void ShowControlPanel()
    {
        controlPanel.SetActive(true);
        pausePanel.SetActive(false);
    }

    public void HideControlPanel()
    {
        controlPanel.SetActive(false);
        pausePanel.SetActive(true);
    }
}
