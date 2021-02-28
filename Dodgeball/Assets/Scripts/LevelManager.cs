using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager S;
    public int currLevel;
    public string currLevelName;
    public GameObject border;
    public GameObject enemySpawner;

    public GameManager.EnemyCountPair[] maxEnemies;

    private void Awake()
    {
        S = this;
    }

    private void Start()
    {
        if (GameManager.S)
        {
            GameManager.S.StartNewGameWrapper();
        }
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToNextLevel()
    {
        SceneManager.LoadScene("Level" + (currLevel + 1));
        currLevel += 1;
    }

}
