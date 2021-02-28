﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public enum GameState { menu, getReady, playing, paused, oops, gameOver };
    public GameState gameState;
    public static GameManager S;

    public GameObject ballPrefab;
    private GameObject currentPlayer;

    public GameObject cinematic;
    private bool seenCinematic = false;

    // UI Variables
    [Header("Basic UI Variables")]
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI endText;
    public GameObject pausePanel;
    public GameObject gameOverPanel;

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
        scoreText.text = "Score: " + 0;
        Time.timeScale = 1;
    }

    private void Update()
    {
        if (gameState == GameState.playing)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                OnPause();
            powerUpTimer += Time.deltaTime;
            if (!powerFilled && powerUpTimer >= 1.0f) powerUpBarFill.fillAmount = Mathf.Clamp(powerUpBarFill.fillAmount - powerUpDecrementRate, 0, 1);

            if (hype >= maxHype)
            {
                audience.GetComponent<Audience>().cheer();
                hype = 0;
            }
            hype -= Time.deltaTime;
            if (hype < 0) hype = 0;
        }

        else if (gameState == GameState.paused && Input.GetKeyDown(KeyCode.Escape)) OnUnpause(); 
        
    }

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

    public void StartNewGame()
    {

        currentPlayer = GameObject.FindGameObjectWithTag("Player");
        spawnPos = currentPlayer.transform.position;
        gameState = GameState.getReady;
        numEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
        powerUpBarFill.fillAmount = 0;
        progressBarFill.fillAmount = 0;

        // TODO: Does enemies number reset or continue?
        foreach (EnemyCountPair p in LevelManager.S.maxEnemies)
        {
            maxEnemies[p.enemyPrefab] = p.enemyCount;
            numEnemiesToSpawn += p.enemyCount;
            currEnemies[p.enemyPrefab.name] = 0;
        }
        totalNumEnemies = numEnemiesToSpawn;
        ResetLevel();
    }

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

    private void ResetLevel()
    {
        hype = 0;
        powerUpTimer = 0;
        powerUpBarFill.fillAmount /= 2;
        powerUpBarFire.enabled = false;
        powerFilled = false;

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
        SoundManager.S.StartWhistle();
        statusText.text = "Go!";
        yield return new WaitForSeconds(1);
        statusText.enabled = false;
        
    }

    private void StartRound()
    {
        gameState = GameState.playing;
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
        Instantiate(enemyPrefab, new Vector3(LevelManager.S.enemySpawner.transform.position.x, Random.Range(-13, 13), 0), Quaternion.identity);
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
        ResetPowerUp(true);
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
        SoundManager.S.GameWinSound();
        statusText.enabled = false;
        gameState = GameState.gameOver;
        endText.text = "Game Won";
        gameOverPanel.SetActive(true);
    }

    public void playerDied()
    {
        if (gameState == GameState.oops) return;
        gameState = GameState.oops;
        StopAllCoroutines();
        ResetPowerUp(false);
        currentPlayer.GetComponent<CapsuleCollider2D>().enabled = false;
        lives -= 1;
        hype = 0;
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

    private IEnumerator betweenRoundsWon()
    {
        statusText.text = LevelManager.S.currLevelName + " Complete!";
        statusText.enabled = true;
        currentPlayer.GetComponent<CapsuleCollider2D>().enabled = false;
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
        IncreaseProgress();
        Debug.Log("Enemies remaining:" + numEnemies + "; on floor: " + numEnemiesOnCourt);
        if (numEnemies <= 0 && numEnemiesToSpawn <= 0)
        {
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

    private void OnPause()
    {
        pausePanel.SetActive(true);
        gameState = GameState.paused;
        Time.timeScale = 0;
    }

    private void OnUnpause()
    {
        pausePanel.SetActive(false);
        gameState = GameState.playing;
        Time.timeScale = 1;
    }

    public bool isPowerFilled()
    {
        return powerFilled;
    }
    
    private void IncreasePower(float increment)
    {
        powerUpBarFill.fillAmount = Mathf.Clamp(powerUpBarFill.fillAmount + increment, 0, 1);
        if (powerUpBarFill.fillAmount >= 1)
        {

            StartCoroutine(Buffed());
        }
    }

    private void IncreaseProgress()
    {
        progressBarFill.fillAmount = (totalNumEnemies - numEnemiesToSpawn) / (float)totalNumEnemies;
    }

    private IEnumerator Buffed()
    {
        powerFilled = true;
        powerUpBarFire.enabled = true;
        currentPlayer.GetComponent<ParticleSystem>().Play();
        currentPlayer.GetComponent<Animator>().SetBool("holding", true);
        yield return new WaitForSeconds(buffDuration);
        ResetPowerUp(true);
    }

    private void ResetPowerUp(bool reset)
    {
        currentPlayer.GetComponent<Animator>().SetBool("holding", false);
        currentPlayer.GetComponent<ParticleSystem>().Stop();
        powerFilled = false;
        powerUpBarFire.enabled = false;
        if (reset) powerUpBarFill.fillAmount = 0;
        powerUpTimer = 0;
    }
}
