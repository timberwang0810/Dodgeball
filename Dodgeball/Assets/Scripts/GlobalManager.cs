using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalManager : MonoBehaviour
{
    public static GlobalManager S;
    private float currentVolume;
    private bool mute;

    private void Awake()
    {
        // Singleton Definition
        if (GlobalManager.S)
        {
            // singleton exists, delete this object
            Destroy(this.gameObject);
        }
        else
        {
            S = this;
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(this);
        currentVolume = 1;
        mute = false;
    }

    private void Update()
    {
        //Debug.Log(mute);
    }

    public void ToggleMute()
    {
        Debug.Log("called");
        mute = !mute;
    }

    public void AdjustVolume(float newVolume)
    {
        currentVolume = newVolume;
    }

    public bool IsMuted()
    {
        return mute;
    }

    public float GetVolume()
    {
        return currentVolume;
    }
}
