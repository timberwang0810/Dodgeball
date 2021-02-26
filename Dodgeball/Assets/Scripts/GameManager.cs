﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public enum GameState { menu, getReady, playing, oops, gameOver };
    public GameState gameState;
    public static GameManager S;

    public GameObject ballPrefab;
    private GameObject currentPlayer;

    // UI Variables
    [Header("Basic UI Variables")]
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI endText;
    public GameObject pausePanel;
    public GameObject gameOverPanel;
    private bool paused;

    [Header("Power Bar")]
    public Slider powerUpBar;
    public Image powerUpBarImage;
    private Color lowPowerUpColor = Color.yellow;
    private Color highPowerUpColor = Color.red;
    public int parryPowerUp;
    public int hitPowerUp;
    public float buffDuration;
    public float timeBetweenBallSpawn;
    public float powerUpDecrementRate;

    [Header("Dodge Bar")]
    public Slider dodgeCoolDownBar;
    public Image dodgeCoolDownBarImage;
    private Color lowCoolDownColor = Color.red;
    private Color highCoolDownColor = Color.green;
    public float dodgeCooldown;

    [Header("Game Variables")]
    public int maxLevel;
    public int lives;
    public int getReadyTime;
    public int maxBallLimit;

    public int maxEnemiesOnCourt;
    public float timeBetweenEnemySpawn;
    
    private int currNumBall;
    private int numEnemies;
    private int score;
    private bool powerFilled;
    private float powerUpTimer;
    private Vector3 spawnPos;
    private Dictionary<GameObject, int> maxEnemies = new Dictionary<GameObject, int>();
    private Dictionary<string, int> currEnemies = new Dictionary<string, int>();
    private int numEnemiesOnCourt;
    private int numEnemiesToSpawn;

    [Header("Audience")]
    public GameObject audience;
    public float maxHype;
    public float enemyPoints;
    public float parryPoints;
    private float hype = 0;

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
        paused = false;
        scoreText.text = "Score: " + 0;

        Time.timeScale = 1;
    }

    private void Update()
    {
        if (gameState == GameState.playing)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                if (paused) OnUnpause();
                else OnPause();
            powerUpTimer += Time.deltaTime;
            if (!powerFilled && powerUpTimer >= 1.0f) powerUpBar.value = Mathf.Clamp(powerUpBar.value - powerUpDecrementRate, 0, 100);

            if (hype >= maxHype)
            {
                audience.GetComponent<Audience>().cheer();
                hype = 0;
            }
            hype -= Time.deltaTime;
            if (hype < 0) hype = 0;
        }
        
    }

    public void StartNewGame()
    {
        currentPlayer = GameObject.FindGameObjectWithTag("Player");
        spawnPos = currentPlayer.transform.position;
        gameState = GameState.getReady;
        numEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
        currNumBall = GameObject.FindGameObjectsWithTag("Ball").Length;

        // TODO: Does enemies number reset or continue?
        foreach (EnemyCountPair p in LevelManager.S.maxEnemies)
        {
            maxEnemies[p.enemyPrefab] = p.enemyCount;
            numEnemiesToSpawn += p.enemyCount;
            currEnemies[p.enemyPrefab.name] = 0;
        }
        ResetLevel();
    }

    private void ResetLevel()
    {
        hype = 0;

        powerUpTimer = 0;
        powerUpBar.minValue = 0;
        powerUpBar.maxValue = 100;
        powerUpBar.value = 0;
        powerUpBarImage.color = lowPowerUpColor;
        powerFilled = false;

        dodgeCoolDownBar.minValue = 0;
        dodgeCoolDownBar.maxValue = dodgeCooldown;
        dodgeCoolDownBar.value = dodgeCooldown;
        dodgeCoolDownBarImage.color = highCoolDownColor;

        StartCoroutine(GetReady());
    }

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
        statusText.text = "Go!";
        yield return new WaitForSeconds(1);
        statusText.enabled = false;
        
    }

    private void StartRound()
    {
        gameState = GameState.playing;
        //StartSpawning();
        StartCoroutine(SpawnEnemies());
    }

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

    private void SpawnOneEnemy()
    {
        if (numEnemiesToSpawn <= 0) return;
        GameObject enemyPrefab = maxEnemies.ElementAt(Random.Range(0, maxEnemies.Count())).Key;
        while (currEnemies[enemyPrefab.name] >= maxEnemies[enemyPrefab])
        {
            enemyPrefab = maxEnemies.ElementAt(Random.Range(0, maxEnemies.Count())).Key;
        }
        // TODO: Instantiate enemy at spawn location
        Instantiate(enemyPrefab, LevelManager.S.enemySpawner.transform);
        currEnemies[enemyPrefab.name] += 1;
        numEnemies++;
        numEnemiesOnCourt++;
        numEnemiesToSpawn--;

        Debug.Log("Current Enemies: " + currEnemies);
        Debug.Log("Enemies Left To Spawn: " + numEnemiesToSpawn);
    }

    private void RoundWon()
    {
        hype = 0;
        gameState = GameState.oops;
        ResetPowerUp();
        if (LevelManager.S.currLevel >= maxLevel)
        {
            GameWon();
        }
        else
        {
            LevelManager.S.GoToNextLevel();
        }
    }

    private void GameWon()
    {
        statusText.enabled = false;
        gameState = GameState.gameOver;
        endText.text = "Game Won";
        gameOverPanel.SetActive(true);
    }

    public void playerDied()
    {
        if (gameState == GameState.oops) return;
        gameState = GameState.oops;
        ResetPowerUp();
        currentPlayer.GetComponent<CapsuleCollider2D>().enabled = false;
        lives -= 1;
        hype = 0;
        if (lives > 0)
        {
            StartCoroutine(betweenRoundsLost());
        }
        else
        {
            StartCoroutine(gameOver());
            gameState = GameState.gameOver;
            
        }
    }

    private IEnumerator betweenRoundsWon()
    {
        statusText.text = LevelManager.S.currLevelName + " Complete!";
        statusText.enabled = true;
        yield return new WaitForSeconds(3);
        currentPlayer.GetComponent<CapsuleCollider2D>().enabled = true;
        RoundWon();
    }

    private IEnumerator gameOver()
    {
        currentPlayer.GetComponent<CapsuleCollider2D>().enabled = false;
        yield return new WaitForSeconds(1);
        currentPlayer.GetComponent<Renderer>().enabled = false;
        gameOverPanel.SetActive(true);
    }

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

    public void OnEnemyDestroyed()
    {
        numEnemies--;
        numEnemiesOnCourt--;
        hype += enemyPoints;
        IncreasePower(hitPowerUp);
        Debug.Log("Enemies remaining:" + numEnemies + "; on floor: " + numEnemiesOnCourt);
        if (numEnemies <= 0 && numEnemiesToSpawn <= 0)
        {
            currentPlayer.GetComponent<CapsuleCollider2D>().enabled = false;
            StartCoroutine(betweenRoundsWon());
        }
    }

    public void OnSuccessfulParry()
    {
        hype += parryPoints;
        IncreasePower(parryPowerUp);
    }

    public void OnScoreAdded(int addedScore)
    {
        score += addedScore;
        scoreText.text = "Score: " + score;
    }

    public void OnBallSpawned()
    {
        currNumBall += 1;
    }

    public void OnBallDespawned()
    {
        currNumBall -= 1;
    }

    private void StartSpawning()
    {
        if (currNumBall < maxBallLimit) StartCoroutine(SpawnBall());
    }

    private IEnumerator SpawnBall()
    {
        yield return new WaitForSeconds(timeBetweenBallSpawn);
        Vector2 spawnLocation = currentPlayer.transform.position;
        GameObject border = LevelManager.S.border;
        // recalculate if distance is too close to player
        while (Vector2.Distance(spawnLocation, currentPlayer.transform.position) <= 8)
        {
            spawnLocation = new Vector2(Random.Range(-23, border.transform.position.x - border.GetComponent<BoxCollider2D>().size.x), Random.Range(-15, 15));
        }
        Instantiate(ballPrefab, spawnLocation, Quaternion.identity);
        currNumBall += 1;
        StartSpawning();
    }

    private void OnPause()
    {
        pausePanel.SetActive(true);
        paused = true;
        Time.timeScale = 0;
    }

    private void OnUnpause()
    {
        pausePanel.SetActive(false);
        paused = false;
        Time.timeScale = 1;
    }

    public bool IsPaused()
    {
        return paused;
    }

    public bool isPowerFilled()
    {
        return powerFilled;
    }

    private void IncreasePower(int increment)
    {
        powerUpBar.value = Mathf.Clamp(powerUpBar.value + increment, 0, 100);
        powerUpBarImage.color = Color.Lerp(lowPowerUpColor, highPowerUpColor, powerUpBar.value / 100);
        if (powerUpBar.value >= 100)
        {
            StartCoroutine(Buffed());
        }
    }

    private IEnumerator Buffed()
    {
        powerFilled = true;
        currentPlayer.GetComponent<ParticleSystem>().Play();
        currentPlayer.GetComponent<Animator>().SetBool("holding", true);
        yield return new WaitForSeconds(buffDuration);
        ResetPowerUp();
    }

    private void ResetPowerUp()
    {
        currentPlayer.GetComponent<Animator>().SetBool("holding", false);
        currentPlayer.GetComponent<ParticleSystem>().Stop();
        powerFilled = false;
        powerUpBar.value = 0;
        powerUpBarImage.color = lowPowerUpColor;
        powerUpTimer = 0;
    }

    public void UpdateDodgeCoolDownBar(float currTimer)
    {
        float clampedTimer = Mathf.Clamp(currTimer, 0, dodgeCooldown);
        dodgeCoolDownBar.value = clampedTimer;
        dodgeCoolDownBarImage.color = Color.Lerp(lowCoolDownColor, highCoolDownColor, clampedTimer / dodgeCooldown);
    }
}
