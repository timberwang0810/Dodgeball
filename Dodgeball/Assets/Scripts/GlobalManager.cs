using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalManager : MonoBehaviour
{
    public static GlobalManager S;

    // Keyboard Bindings
    [Header("Keyboard Bindings")]
    public KeyCode currParryKeyCode;
    public KeyCode currThrowKeyCode;
    public KeyCode currDodgeKeyCode;

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
        currentVolume = 1; //default
        mute = false;
    }

    public void ToggleMute()
    {
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
