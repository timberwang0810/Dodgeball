using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public enum GameState { menu, getReady, playing, oops, gameOver };
    public GameState gameState;
    public static GameManager S;

    public GameObject currentPlayer;
    public GameObject ballPrefab;
    public GameObject border;

    public TextMeshProUGUI statusText;

    public int lives;
    public int getReadyTime;
    public int maxBallLimit;
    public float timeBetweenBallSpawn;
    private int currNumBall;

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
        spawnPos = currentPlayer.transform.position;
        Cursor.visible = true;
        currNumBall = GameObject.FindGameObjectsWithTag("Ball").Length;
        StartNewGame();
    }

    private void StartNewGame()
    {
        gameState = GameState.getReady;
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

    public void playerDied()
    {
        gameState = GameState.oops;
        currentPlayer.GetComponent<Renderer>().enabled = false;
        currentPlayer.GetComponent<CapsuleCollider2D>().enabled = false;
        lives -= 1;
        if (lives > 0)
        {
            StartCoroutine(betweenRounds());
        }
        else
        {
            gameState = GameState.gameOver;
            statusText.text = "Game Over";
            statusText.enabled = true;
        }
    }

    private IEnumerator betweenRounds()
    {
        
        statusText.text = "Lives Left:" + lives;
        statusText.enabled = true;
        yield return new WaitForSeconds(1);
        currentPlayer.GetComponent<Renderer>().enabled = true;
        currentPlayer.GetComponent<CapsuleCollider2D>().enabled = true;
        currentPlayer.transform.position = spawnPos;
        StartNewGame();
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
        // recalculate if distance is too close to player
        while (Vector2.Distance(spawnLocation, currentPlayer.transform.position) <= 8)
        {
            spawnLocation = new Vector2(Random.Range(-15, 15), Random.Range(-23, border.transform.position.y - border.GetComponent<BoxCollider2D>().size.y));
        }
        Instantiate(ballPrefab, spawnLocation, Quaternion.identity);
        currNumBall += 1;
        StartSpawning();
    }
}
