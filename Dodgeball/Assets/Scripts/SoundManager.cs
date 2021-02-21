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

    public AudioClip ballThrow1;

    public AudioClip dodge;

    private AudioSource audio;
    public AudioSource bgm;

    public Button muteButton;
    public Button unmuteButton;

    private int newRandomNumber;
    private int lastRandomNumber;
        


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

    public void HitSound()
    {
        int r = lastRandomNumber;
        newRandomNumber = Random.Range(1, 12);
        if (newRandomNumber != lastRandomNumber)
        {
            r = newRandomNumber;
            newRandomNumber = lastRandomNumber;
            if (r == 1)
            {
                audio.PlayOneShot(hit1);
            }
            else if (r == 2)
            {
                audio.PlayOneShot(hit2);
            }
            else if (r == 3)
            {
                audio.PlayOneShot(hit3);
            }
            else if (r == 4)
            {
                audio.PlayOneShot(hit4);
            }
            else if (r == 5)
            {
                audio.PlayOneShot(hit5);
            }
            else if (r == 6)
            {
                audio.PlayOneShot(hit6);
            }
            else if (r == 7)
            {
                audio.PlayOneShot(hit7);
            }
            else if (r == 8)
            {
                audio.PlayOneShot(hit8);
            }
            else if (r == 9)
            {
                audio.PlayOneShot(hit9);
            }
            else if (r == 10)
            {
                audio.PlayOneShot(hit10);
            }
            else if (r == 11)
            {
                audio.PlayOneShot(hit11);
            }
            else
            {
                audio.PlayOneShot(hit12);
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
