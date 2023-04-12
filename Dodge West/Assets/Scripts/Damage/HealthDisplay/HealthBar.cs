using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Progressive _health;   // Health object (used to manage health amount)
    [SerializeField] private Image _fillImage;      // Image for the health bar
    [SerializeField] private Gradient _gradient;    // Used to determine the Colour of the health bar

    private Camera _cam;
    //[SerializeField] private float reduceSpeed = 2;

    private void Start()
    {
        //_cam = Camera.main;
        _fillImage.color = _gradient.Evaluate(_health.Ratio);

        //_fillImage.transform.rotation = Quaternion.LookRotation(transform.position - _cam.transform.position);
    }

    //private void Update()
    //{
    //    //_fillImage.transform.rotation = Quaternion.LookRotation(transform.position - _cam.transform.position);
    //    //_fillImage.fillAmount = Mathf.MoveTowards(_fillImage.fillAmount, _health.Ratio, reduceSpeed * Time.deltaTime);
    //}

    // The two functions below update the health bar when the health object invokes the "OnChange" action variable

    private void OnEnable()
    {
        _health.OnChange += UpdateBar;
    }

    private void OnDisable()
    {
        _health.OnChange -= UpdateBar;
    }

    // Changes the fill amount of the bar and the color based on the health objects' ratio
    private void UpdateBar()
    {
        _fillImage.fillAmount = _health.Ratio;
        _fillImage.color = _gradient.Evaluate(_health.Ratio);

        //_fillImage.fillAmount = Mathf.MoveTowards(_fillImage.fillAmount, _health.Ratio, reduceSpeed * Time.deltaTime);
    }
}
