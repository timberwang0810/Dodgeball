using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ControlManager : MonoBehaviour
{
    public static ControlManager S;
    public enum GameControl { Parry, Throw, Dodge }

    // Keyboard Bindings
    [Header("Keyboard Bindings")]
    public KeyCode currParryKeyCode;
    public KeyCode currThrowKeyCode;
    public KeyCode currDodgeKeyCode;

    public bool isBindingEditing = false;

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
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.S.gameState != GameManager.GameState.paused) isBindingEditing = false;
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

    public void ToggleEditing()
    {
        isBindingEditing = !isBindingEditing;
    }

    private void DetectKey()
    {
        foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(key) && key != KeyCode.Escape)
            {
                if (isParrySelected) currParryKeyCode = key;
                else if (isThrowSelected) currThrowKeyCode = key;
                else if (isDodgeSelected) currDodgeKeyCode = key;
                else return;
                if (currButtonText) currButtonText.text = key.ToString();
                isBindingEditing = false;
            }
        }
    }
}
