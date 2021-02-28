using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    public void btn_StartTheGame()
    {
        Time.timeScale = 1;
        if (GameManager.S)
        {
            Destroy(GameManager.S.gameObject);
        }
        SceneManager.LoadScene("Level1");
        Destroy(this.gameObject, 1.0f);
    }
    public IEnumerator StartTheGame()
    {
        yield return new WaitForSeconds(.7f);
        SceneManager.LoadScene("Level1");
    }

    public void btn_Instructions()
    {
        SceneManager.LoadScene("Instructions");
        Destroy(this.gameObject, 1.0f);
    }
    public IEnumerator Instructions()
    {
        yield return new WaitForSeconds(.7f);
        SceneManager.LoadScene("Instructions");
    }

    public void btn_Credits()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Credits");
        Destroy(this.gameObject, 1.0f);
    }
    public IEnumerator Credits()
    {
        yield return new WaitForSeconds(.7f);
        SceneManager.LoadScene("Credits");
    }

    public void btn_QuitGame()
    {
        Time.timeScale = 1;
        Application.Quit();
        Destroy(this.gameObject, 1.0f);
    }
    public IEnumerator Quit()
    {
        yield return new WaitForSeconds(.7f);
        Application.Quit();
    }

    public void btn_Back()
    {
        Time.timeScale = 1;
        if (GameManager.S)
        {
            Destroy(GameManager.S.gameObject);
        }
        SceneManager.LoadScene("Title");
        Destroy(this.gameObject, 1.0f);
    }
    public IEnumerator Back()
    {
        yield return new WaitForSeconds(.7f);
        SceneManager.LoadScene("Title");
    }
}
