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

    public TextMeshProUGUI statusText;

    public int lives;
    public int getReadyTime;

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
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
