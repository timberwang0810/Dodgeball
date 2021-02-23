using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{

    public static SoundManager S;

    public AudioClip hit1;
    public AudioClip hit2;
    public AudioClip hit3;
    public AudioClip hit4;
    public AudioClip hit5;
    public AudioClip hit6;
    public AudioClip hit7;
    public AudioClip hit8;
    public AudioClip hit9;
    public AudioClip hit10;
    public AudioClip hit11;
    public AudioClip hit12;

    public AudioClip wallHit1;
    public AudioClip wallHit2;
    public AudioClip wallHit3;
    public AudioClip wallHit4;
    public AudioClip wallHit5;

    public AudioClip Kevin1;
    public AudioClip Kevin2;
    public AudioClip Kevin3;
    public AudioClip Kevin4;

    public AudioClip Jim1;
    public AudioClip Jim2;
    public AudioClip Jim3;
    public AudioClip Jim4;

    public AudioClip KevinIdle;
    public AudioClip JimIdle;

    public AudioClip KevinWin;
    public AudioClip JimWin;
    public AudioClip CrowdWin1;
    public AudioClip CrowdWin2;

    public AudioClip Crowdyell1;
    public AudioClip Crowdyell2;
    public AudioClip Crowd1;
    public AudioClip Crowd2;
    public AudioClip Crowd3;
    public AudioClip Crowd4;


    public AudioClip ballThrow1;

    public AudioClip dodge;

    private AudioSource audio;
    public AudioSource bgm;

    public Button muteButton;
    public Button unmuteButton;

    private int newRandomNumber;
    private int lastRandomNumber;
    private int newRandomNumber4;
    private int lastRandomNumber4; 
    private int newRandomNumber2;
    private int lastRandomNumber2;


    private void Awake()
    {
        S = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    public void playMusic()
    {
        bgm.Play();
        muteButton.gameObject.SetActive(true);
        unmuteButton.gameObject.SetActive(false);
    }

    public void stopMusic()
    {
        bgm.Stop();
        muteButton.gameObject.SetActive(false);
        unmuteButton.gameObject.SetActive(true);
    }

    public IEnumerator KevinCommentary()
    {
        int t = lastRandomNumber4;
        newRandomNumber4 = Random.Range(1, 4);
        if (newRandomNumber4 != lastRandomNumber4)
        {
            t = newRandomNumber4;
            lastRandomNumber4 = newRandomNumber4;
            if (t == 1)
            {
                audio.PlayOneShot(Kevin1);
                audio.PlayOneShot(Crowd1);
                audio.PlayOneShot(Crowdyell1, 0.8f);
                yield return new WaitForSeconds(1.5f);
                JimCommentary();
            }
            else if (t == 2)
            {
                audio.PlayOneShot(Kevin2);
                audio.PlayOneShot(Crowd2);
                audio.PlayOneShot(Crowdyell1, 0.8f);

                yield return new WaitForSeconds(2.5f);
                JimCommentary();

            }
            else if (t == 3)
            {
                audio.PlayOneShot(Kevin3);
                audio.PlayOneShot(Crowd3);
                audio.PlayOneShot(Crowdyell2, 0.8f);

                yield return new WaitForSeconds(3.0f);
                JimCommentary();

            }
            else
            {
                audio.PlayOneShot(Kevin4);
                audio.PlayOneShot(Crowd4);
                audio.PlayOneShot(Crowdyell2, 0.8f);

                yield return new WaitForSeconds(2.0f);
                JimCommentary();

            }
        }
        else
        {
            StartCoroutine(KevinCommentary());
        }

    }

    public void JimCommentary()
    {
        int y = lastRandomNumber2;
        newRandomNumber2 = Random.Range(1, 4);
        if (newRandomNumber2 != lastRandomNumber2)
        {
            y = newRandomNumber4;
            lastRandomNumber2 = newRandomNumber2;
            if (y == 1)
            {
                audio.PlayOneShot(Jim1);
            }
            else if (y == 2)
            {
                audio.PlayOneShot(Jim2);
            }
            else if (y == 3)
            {
                audio.PlayOneShot(Jim3);
            }
            else
            {
                audio.PlayOneShot(Jim4);
            }
        }
        else
        {
            JimCommentary();
        }
    }
    public void HitSound()
    {
        int r = lastRandomNumber;
        newRandomNumber = Random.Range(1, 12);
        if (newRandomNumber != lastRandomNumber)
        {
            r = newRandomNumber;
            lastRandomNumber = newRandomNumber;
            if (r == 1)
            {
                audio.PlayOneShot(hit1);
                StartCoroutine(KevinCommentary());
            }
            else if (r == 2)
            {
                audio.PlayOneShot(hit2);
                StartCoroutine(KevinCommentary());

            }
            else if (r == 3)
            {
                audio.PlayOneShot(hit3);
                StartCoroutine(KevinCommentary());

            }
            else if (r == 4)
            {
                audio.PlayOneShot(hit4);
                StartCoroutine(KevinCommentary());

            }
            else if (r == 5)
            {
                audio.PlayOneShot(hit5);
                StartCoroutine(KevinCommentary());

            }
            else if (r == 6)
            {
                audio.PlayOneShot(hit6);
                StartCoroutine(KevinCommentary());

            }
            else if (r == 7)
            {
                audio.PlayOneShot(hit7);
                StartCoroutine(KevinCommentary());

            }
            else if (r == 8)
            {
                audio.PlayOneShot(hit8);
                StartCoroutine(KevinCommentary());

            }
            else if (r == 9)
            {
                audio.PlayOneShot(hit9);
                StartCoroutine(KevinCommentary());

            }
            else if (r == 10)
            {
                audio.PlayOneShot(hit10);
                StartCoroutine(KevinCommentary());

            }
            else if (r == 11)
            {
                audio.PlayOneShot(hit11);
                StartCoroutine(KevinCommentary());

            }
            else
            {
                audio.PlayOneShot(hit12);
                StartCoroutine(KevinCommentary());

            }
        }
        else
        {
            HitSound();
        }


    }

    public void DodgeSound()
    {
        audio.PlayOneShot(dodge);
    }

    public void WallHitSound()
    {
        int r = Random.Range(1, 5);
        if (r == 1)
        {
            audio.PlayOneShot(wallHit1);
        }
        else if (r == 2)
        {
            audio.PlayOneShot(wallHit2);
        }
        else if (r == 3)
        {
            audio.PlayOneShot(wallHit3);
        }
        else if (r == 4)
        {
            audio.PlayOneShot(wallHit4);
        }
        else
        {
            audio.PlayOneShot(wallHit5);
        }

    }

    public void ThrowSound()
    {
        audio.PlayOneShot(ballThrow1);
    }


}
