using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ChoiceButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text titleOfChoice;

    public void Init(string title, UnityAction onClickEvent)
    {
        button.onClick.AddListener(onClickEvent);
        titleOfChoice.text = title;
    }
}
