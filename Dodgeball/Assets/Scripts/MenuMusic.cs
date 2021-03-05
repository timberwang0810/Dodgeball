using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMusic : MonoBehaviour
{
    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = GlobalManager.S.GetVolume();
        if (GlobalManager.S.IsMuted()) audioSource.volume = 0;
    }
}
