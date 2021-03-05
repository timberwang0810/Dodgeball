using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public Slider volumeSlider;
    public Toggle muteToggle;
  
    // Start is called before the first frame update
    void Start()
    {
        volumeSlider.value = GlobalManager.S.GetVolume();
        muteToggle.isOn = GlobalManager.S.IsMuted();
        if (GlobalManager.S.IsMuted()) volumeSlider.enabled = false;
        else volumeSlider.enabled = true;

        Debug.Log("Muted? " + GlobalManager.S.IsMuted());
    }

    public void ToggleMute()
    {
        if (muteToggle.isOn == GlobalManager.S.IsMuted()) return;
        GlobalManager.S.ToggleMute();
        if (GlobalManager.S.IsMuted()) volumeSlider.enabled = false;
        else volumeSlider.enabled = true;
    }

    public void AdjustVolume()
    {
        GlobalManager.S.AdjustVolume(volumeSlider.value);
    }
}
