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
    public AudioClip hitNerd1;
    public AudioClip hitNerd2;
    public AudioClip hitNerd3;
    public AudioClip hitNerd4;
    public AudioClip hitNerd5;
    public AudioClip hitNerd6;
    public AudioClip hitNerd7;
    public AudioClip hitNerd8;

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

    public AudioClip Idle1;
    public AudioClip Idle2;

    public AudioClip CrowdWin1;
    public AudioClip CrowdWin2;

    public AudioClip Crowdyell1;
    public AudioClip Crowdyell2;
    public AudioClip Crowd1;
    public AudioClip Crowd2;
    public AudioClip Crowd3;
    public AudioClip Crowd4;

    public AudioClip playerHit1;
    public AudioClip playerHit2;
    public AudioClip playerHit3;
    public AudioClip playerHit4;
    public AudioClip playerParry1;
    public AudioClip playerParry2;
    public AudioClip playerParry3;

    public GameObject poweredUp;
    public AudioClip KevinDash;
    public AudioClip JimDash;

    public AudioClip KevinWin1;
    public AudioClip KevinWin2;
    public AudioClip JimWin1;
    public AudioClip JimWin2;

    public AudioClip KevinLose;
    public AudioClip JimLose;

    public AudioClip KevinIntro1;
    public AudioClip KevinIntro2;
    public AudioClip JimIntro1;
    public AudioClip JimStart;
    public AudioClip KevinStart;

    public AudioClip WinJingle;
    public AudioClip Whistle;
    public AudioClip LoseJingle;

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
    private int lastRandomIdle;
    private int newRandomStart;
    private int lastRandomStart = 0;

    private int newPlayerSound;
    private int lastPlayerSound;

    [Header("Commentary cooldown")]
    public float cooldown;
    private float cooldownTimer = 0;

    private void Awake()
    {
        S = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    void Update()
    {
        cooldownTimer += Time.deltaTime;
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
                yield return new WaitForSeconds(1.2f);
                JimCommentary();
            }
            else if (t == 2)
            {
                audio.PlayOneShot(Kevin2);
                audio.PlayOneShot(Crowd2);
                audio.PlayOneShot(Crowdyell1, 0.8f);

                yield return new WaitForSeconds(2.0f);
                JimCommentary();

            }
            else if (t == 3)
            {
                audio.PlayOneShot(Kevin3);
                audio.PlayOneShot(Crowd3);
                audio.PlayOneShot(Crowdyell2, 0.8f);

                yield return new WaitForSeconds(1.5f);
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

    private void PlayCrowd()
    {
        int randomCrowd = Random.Range(1, 6);
        int randomCrowdYell = Random.Range(1, 3);
        if (randomCrowd == 1)
        {
            audio.PlayOneShot(Crowd1);
        }
        else if (randomCrowd == 2)
        {
            audio.PlayOneShot(Crowd2);
        }
        else if (randomCrowd == 3)
        {
            audio.PlayOneShot(Crowd3);
        }
        else if (randomCrowd == 4)
        {
            audio.PlayOneShot(Crowd4);
        }

        if (randomCrowdYell == 1)
        {
            audio.PlayOneShot(Crowdyell1, 0.8f);
        }
        else if (randomCrowdYell == 2)
        {
            audio.PlayOneShot(Crowdyell2, 0.8f);
        }
    }

    private void HitSoundNoCommentary()
    {
        int r = lastRandomNumber;
        newRandomNumber = Random.Range(1, 8);
        PlayCrowd();
        if (newRandomNumber != lastRandomNumber)
        {
            r = newRandomNumber;
            lastRandomNumber = newRandomNumber;
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
            else
            {
                audio.PlayOneShot(hit8);
            }
        }
        else
        {
            HitSound();
        }
    }
    private void HitSoundNerdsNoCommentary()
    {
        int r = lastRandomNumber;
        newRandomNumber = Random.Range(1, 8);
        PlayCrowd();
        if (newRandomNumber != lastRandomNumber)
        {
            r = newRandomNumber;
            lastRandomNumber = newRandomNumber;
            if (r == 1)
            {
                audio.PlayOneShot(hitNerd1);
            }
            else if (r == 2)
            {
                audio.PlayOneShot(hitNerd2);
            }
            else if (r == 3)
            {
                audio.PlayOneShot(hitNerd3);
            }
            else if (r == 4)
            {
                audio.PlayOneShot(hitNerd4);
            }
            else if (r == 5)
            {
                audio.PlayOneShot(hitNerd5);
            }
            else if (r == 6)
            {
                audio.PlayOneShot(hitNerd6);
            }
            else if (r == 7)
            {
                audio.PlayOneShot(hitNerd7);
            }
            else
            {
                audio.PlayOneShot(hitNerd8);
            }
        }
        else
        {
            HitSoundNerds();
        }
    }
    public void HitSound()
    {
        int r = lastRandomNumber;
        newRandomNumber = Random.Range(1, 8);
        if (newRandomNumber != lastRandomNumber)
        {
            if (cooldownTimer >= cooldown)
            {
                StartCoroutine(KevinCommentary());
                cooldownTimer = 0;
            }
            else
            {
                HitSoundNoCommentary();
                return;
            }
            r = newRandomNumber;
            lastRandomNumber = newRandomNumber;
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
            else
            {
                audio.PlayOneShot(hit8);
            }
        }
        else
        {
            HitSound();
        }


    }
    public void HitSoundNerds()
    {
        int r = lastRandomNumber;
        newRandomNumber = Random.Range(1, 8);
        if (newRandomNumber != lastRandomNumber)
        {
            if (cooldownTimer >= cooldown)
            {
                StartCoroutine(KevinCommentary());
                cooldownTimer = 0;
            }
            else
            {
                HitSoundNerdsNoCommentary();
                return;
            }
            r = newRandomNumber;
            lastRandomNumber = newRandomNumber;
            if (r == 1)
            {
                audio.PlayOneShot(hitNerd1);
            }
            else if (r == 2)
            {
                audio.PlayOneShot(hitNerd2);

            }
            else if (r == 3)
            {
                audio.PlayOneShot(hitNerd3);
            }
            else if (r == 4)
            {
                audio.PlayOneShot(hitNerd4);
            }
            else if (r == 5)
            {
                audio.PlayOneShot(hitNerd5);
            }
            else if (r == 6)
            {
                audio.PlayOneShot(hitNerd6);
            }
            else if (r == 7)
            {
                audio.PlayOneShot(hitNerd7);
            }
            else
            {
                audio.PlayOneShot(hitNerd8);
            }
        }
        else
        {
            HitSoundNerds();
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

    public void PlayerHitSound()
    {
        int playerHitNumber = lastPlayerSound;
        newPlayerSound = Random.Range(1, 4);
        if (newPlayerSound != lastPlayerSound)
        {
            playerHitNumber = newPlayerSound;
            lastPlayerSound = newPlayerSound;
            if (playerHitNumber == 1)
            {
                audio.PlayOneShot(playerHit1, 0.5f);
            }
            else if (playerHitNumber == 2)

            {
                audio.PlayOneShot(playerHit2, 0.5f);

            }
            else if (playerHitNumber == 3)

            {
                audio.PlayOneShot(playerHit3, 0.5f);

            }
            else
            {
                audio.PlayOneShot(playerHit4, 0.5f);
            }
        }
        else
        {
            StartCoroutine(KevinCommentary());
        }
    }


    public void TooManyDashSound()
    {
        audio.PlayOneShot(KevinDash);
        StartCoroutine(JimDashSound());
    }

    public IEnumerator JimDashSound()
    {
        yield return new WaitForSeconds(2.0f);
        audio.PlayOneShot(JimDash);
    }

    public void StartWhistle()
    {
        audio.PlayOneShot(Whistle, 0.3f);
        StartCoroutine(StartCommentary());
    }

    public IEnumerator StartCommentary()
    {
        int r = lastRandomStart;
        newRandomStart = Random.Range(0,4);
        if (newRandomStart != lastRandomStart)
        {
            yield return new WaitForSeconds(0.7f);
            r = newRandomStart;
            lastRandomStart = newRandomStart;
            if (r >= 2)
            {
                audio.PlayOneShot(JimStart);
            }
            else
            {
                audio.PlayOneShot(KevinStart);

            }
        }
        else
        {
            StartCoroutine(StartCommentary());
        }
    }

    public void GameWinSound()
    {
        audio.PlayOneShot(WinJingle, 0.3f);
        StartCoroutine(WinCommentary());

    }

    public IEnumerator WinCommentary()
    {
        yield return new WaitForSeconds(0.5f);
        int RandomWin = Random.Range(1, 2);
        if(RandomWin == 1)
        {
            audio.PlayOneShot(KevinWin1);
            audio.PlayOneShot(JimWin1);
        }
        else
        {
            audio.PlayOneShot(KevinWin2);
            audio.PlayOneShot(JimWin2);
        }


    }
    public void GameLoseSound()
    {
        audio.PlayOneShot(LoseJingle, 0.3f);
        StartCoroutine(LoseCommentary());

    }

    public IEnumerator LoseCommentary()
    {
        yield return new WaitForSeconds(0.5f);
        audio.PlayOneShot(KevinLose);
        yield return new WaitForSeconds(6.0f);
        audio.PlayOneShot(JimLose);


    }

    public void ParrySound()
    {
        int randomParrySound = Random.Range(0, 2);
        if (randomParrySound == 0)
        {
            audio.PlayOneShot(playerParry1, 0.5f);
        }
        else if (randomParrySound == 1)
        {
            audio.PlayOneShot(playerParry2, 0.5f);
        }
        else
        {
            audio.PlayOneShot(playerParry3, 0.5f);

        }


    }

    public void IdleCommentary()
    {
        int randomIdle = lastRandomIdle;
        int newRandomIdle = Random.Range(0,1);
        if (newRandomNumber != lastRandomIdle)
        {

            if (randomIdle == 0)
            {
                audio.PlayOneShot(Idle1);
                newRandomIdle = lastRandomIdle;
            }
            else
            {
                audio.PlayOneShot(Idle2);
                newRandomIdle = lastRandomIdle;

            }
        }
        else
        {
            IdleCommentary();
        }
    }

    public void PoweredUpSound()
    {
        poweredUp.GetComponent<AudioSource>().Play();
    }

    public void StopPoweredUpSound()
    {
        poweredUp.GetComponent<AudioSource>().Stop();
    }
}
