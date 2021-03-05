using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ControlManager : MonoBehaviour
{
    public static ControlManager S;

    public bool isBindingEditing = false;

    public Button ParryButton;
    public Button ThrowButton;
    public Button DodgeButton;

    private TextMeshProUGUI currButtonText;

    private bool isParrySelected;
    private bool isThrowSelected;
    private bool isDodgeSelected;

    private void Awake()
    {
        // Singleton Definition
        if (ControlManager.S)
        {
            // singleton exists, delete this object
            Destroy(this.gameObject);
        }
        else
        {
            S = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
        isBindingEditing = false;
        ParryButton.GetComponentInChildren<TextMeshProUGUI>().text = GlobalManager.S.currParryKeyCode.ToString();
        ThrowButton.GetComponentInChildren<TextMeshProUGUI>().text = GlobalManager.S.currThrowKeyCode.ToString();
        DodgeButton.GetComponentInChildren<TextMeshProUGUI>().text = GlobalManager.S.currDodgeKeyCode.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.S && GameManager.S.gameState != GameManager.GameState.paused) isBindingEditing = false;
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
                if (isParrySelected) GlobalManager.S.currParryKeyCode = key;
                else if (isThrowSelected) GlobalManager.S.currThrowKeyCode = key;
                else if (isDodgeSelected) GlobalManager.S.currDodgeKeyCode = key;
                else return;
                if (currButtonText) currButtonText.text = key.ToString();
                isBindingEditing = false;
            }
        }
    }
}
