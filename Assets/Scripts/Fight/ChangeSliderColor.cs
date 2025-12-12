using UnityEngine;
using UnityEngine.UI;

namespace Fight
{
    public class ChangeSliderColor : MonoBehaviour
    {
        [SerializeField] private Slider sliderRef;
        [SerializeField] private Image fillArea;
        [SerializeField] private Color startColor;
        [SerializeField] private Color endColor;
        [SerializeField] private float maxSliderValue = 1;
        private void Awake()
        {
            if (sliderRef != null)
            {
                sliderRef.onValueChanged.AddListener(OnSliderValueChanged);
            }
        }

        private void OnSliderValueChanged(float value)
        {
            fillArea.color = Color.Lerp(startColor, endColor, value / maxSliderValue);
        }
    }
}
