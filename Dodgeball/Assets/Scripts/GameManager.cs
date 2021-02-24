using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public enum GameState { menu, getReady, playing, oops, gameOver };
    public GameState gameState;
    public static GameManager S;

    public GameObject ballPrefab;

    public TextMeshProUGUI statusText;
    public TextMeshProUGUI scoreText;
    public GameObject pausePanel;
    private bool paused;
    private GameObject currentPlayer;

    public int maxLevel;

    public int lives;
    public int getReadyTime;
    public int maxBallLimit;
    public float timeBetweenBallSpawn;
    private int currNumBall;
    private int numEnemies;
    private int score;

    private Vector3 spawnPos;

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
        currNumBall = GameObject.FindGameObjectsWithTag("Ball").Length;
        pausePanel.SetActive(false);
        paused = false;
        scoreText.text = "Score: " + 0;
        Debug.Log("Screen Width : " + Screen.width);
        Time.timeScale = 1;
    }

    private void Update()
    {
        if (gameState == GameState.playing)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                if (paused) OnUnpause();
                else OnPause();
        }
        
    }

    public void StartNewGame()
    {
        currentPlayer = GameObject.FindGameObjectWithTag("Player");
        spawnPos = currentPlayer.transform.position;
        gameState = GameState.getReady;
        numEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
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
        StartSpawning();
    }

    private void RoundWon()
    {
        gameState = GameState.oops;
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
        gameState = GameState.gameOver;
        statusText.text = "Game Won";
        statusText.enabled = true;
    }

    public void playerDied()
    {
        if (gameState == GameState.oops) return;
        gameState = GameState.oops;
        currentPlayer.GetComponent<Renderer>().enabled = false;
        currentPlayer.GetComponent<CapsuleCollider2D>().enabled = false;
        lives -= 1;
        if (lives > 0)
        {
            StartCoroutine(betweenRoundsLost());
        }
        else
        {
            gameState = GameState.gameOver;
            statusText.text = "Game Over";
            statusText.enabled = true;
        }
    }

    private IEnumerator betweenRoundsWon()
    {
        statusText.text = LevelManager.S.currLevelName + " Complete!";
        statusText.enabled = true;
        yield return new WaitForSeconds(3);
        RoundWon();
    }

    private IEnumerator betweenRoundsLost()
    {
        
        statusText.text = "Lives Left:" + lives;
        statusText.enabled = true;
        yield return new WaitForSeconds(1);
        currentPlayer.GetComponent<Renderer>().enabled = true;
        currentPlayer.GetComponent<CapsuleCollider2D>().enabled = true;
        currentPlayer.transform.position = spawnPos;
        StartNewGame();
    }

    public void OnEnemyDestroyed()
    {
        numEnemies--;
        if (numEnemies <= 0) StartCoroutine(betweenRoundsWon());
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
}
