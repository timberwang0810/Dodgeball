using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    public Slider volumeSlider;
    public Toggle muteToggle;

    public bool isBindingEditing = false;

    public Button ParryButton;
    public Button ThrowButton;
    public Button DodgeButton;

    private TextMeshProUGUI currButtonText;

    private bool isParrySelected = false;
    private bool isThrowSelected = false;
    private bool isDodgeSelected = false;

    // Start is called before the first frame update
    void Start()
    {
        volumeSlider.value = GlobalManager.S.GetVolume();
        muteToggle.isOn = GlobalManager.S.IsMuted();
        if (GlobalManager.S.IsMuted()) volumeSlider.enabled = false;
        else volumeSlider.enabled = true;

        ParryButton.GetComponentInChildren<TextMeshProUGUI>().text = GlobalManager.S.currParryKeyCode.ToString();
        ThrowButton.GetComponentInChildren<TextMeshProUGUI>().text = GlobalManager.S.currThrowKeyCode.ToString();
        DodgeButton.GetComponentInChildren<TextMeshProUGUI>().text = GlobalManager.S.currDodgeKeyCode.ToString();
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

    // Update is called once per frame
    void Update()
    {
        if (isBindingEditing) DetectKey();
    }

    public void OnParryControlPressed(TextMeshProUGUI text)
    {
        if (!isBindingEditing)
        {
            isBindingEditing = true;
            isParrySelected = true;
            isThrowSelected = false;
            isDodgeSelected = false;
            currButtonText = text;
        }
    }

    public void OnThrowControlPressed(TextMeshProUGUI text)
    {
        if (!isBindingEditing)
        {
            isBindingEditing = true;
            isParrySelected = false;
            isThrowSelected = true;
            isDodgeSelected = false;
            currButtonText = text;
        }
    }

    public void OnDodgeControlPressed(TextMeshProUGUI text)
    {
        if (!isBindingEditing)
        {
            isBindingEditing = true;
            isParrySelected = false;
            isThrowSelected = false;
            isDodgeSelected = true;
            currButtonText = text;
        }
    }

    private void DetectKey()
    {
        foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(key) && key != KeyCode.Escape)
            {
                if (isParrySelected && (key != GlobalManager.S.currThrowKeyCode && key != GlobalManager.S.currDodgeKeyCode)) GlobalManager.S.currParryKeyCode = key;
                else if (isThrowSelected && (key != GlobalManager.S.currParryKeyCode && key != GlobalManager.S.currDodgeKeyCode)) GlobalManager.S.currThrowKeyCode = key;
                else if (isDodgeSelected && (key != GlobalManager.S.currThrowKeyCode && key != GlobalManager.S.currParryKeyCode)) GlobalManager.S.currDodgeKeyCode = key;
                else return;
                if (currButtonText) currButtonText.text = key.ToString();
                isBindingEditing = false;
            }
        }
    }


}
