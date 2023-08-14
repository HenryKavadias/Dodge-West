using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Controls the UI display for player lifes
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
            UpdateUI();
        }
    }

    // Two functions below add the UpdateUI function to the OnCountChange action variable
    private void OnEnable()
    {
        _lives.OnCountChange += UpdateUI;
    }

    private void OnDisable()
    {
        _lives.OnCountChange -= UpdateUI;
    }

    private void UpdateUI()
    {
        _livesTextDisplay.text = _lives.Current.ToString();

        // Put UI updates here
        gameObject.GetComponent<CameraManager>().UpdateLifeUI(_lives.Current);
    }
}
