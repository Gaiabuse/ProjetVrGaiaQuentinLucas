using System;
using UnityEngine;
using UnityEngine.UI;

public class ChangeSliderColor : MonoBehaviour
{
    [SerializeField] private Slider sliderRef;
    [SerializeField] private Image fillArea;
    [SerializeField] private Color startColor;
    [SerializeField] private Color endColor;
    private void Awake()
    {
        if (sliderRef != null)
        {
            sliderRef.onValueChanged.AddListener(OnSliderValueChanged); // je sais que ça se fait aussi directement dans l'inspecteur mai splus opti et flexible comme ça
        }
    }

    private void OnSliderValueChanged(float value)
    {
        fillArea.color = Color.Lerp(startColor, endColor, value);
    }
}
