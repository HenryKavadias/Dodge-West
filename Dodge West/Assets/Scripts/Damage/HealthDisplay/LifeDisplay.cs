using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 
public class LifeDisplay : MonoBehaviour
{
    [SerializeField] private LifeCounter _lives;
    [SerializeField] private TextMeshProUGUI _livesTextDisplay;

    public void SetLifeCounter(LifeCounter lives)
    {
        _lives = lives;
    }

    public void SetTextDisplay(TextMeshProUGUI textDisplay)
    {
        _livesTextDisplay = textDisplay;
    }

    public void Start()
    {
        if (_livesTextDisplay)
        {
            UpdateText();
        }
    }

    // Two functions below add the UpdateText function to the OnCountChange action variable
    private void OnEnable()
    {
        _lives.OnCountChange += UpdateText;
    }

    private void OnDisable()
    {
        _lives.OnCountChange -= UpdateText;
    }

    private void UpdateText()
    {
        _livesTextDisplay.text = _lives.Current.ToString();
    }
}
