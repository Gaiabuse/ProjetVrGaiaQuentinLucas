using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject infoPanel;
    public Image objectImageUI;
    public TMP_Text objectTextUI;

    void Awake()
    {
        instance = this;
        infoPanel.SetActive(false);
    }

    public void ShowInfo(Sprite image, string text)
    {
        objectImageUI.sprite = image;
        objectTextUI.text = text;
        infoPanel.SetActive(true);
    }

    public void HideInfo()
    {
        infoPanel.SetActive(false);
    }
}