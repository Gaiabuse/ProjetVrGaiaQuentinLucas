using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;

public class ChoiceButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text titleOfChoice;
    [SerializeField] private XRController[] allKeysForChoice;
    [SerializeField] private Sprite[] visualKeysSprites;
    private UnityEvent choiceButtonPressed;
    private XRController keyForChoice;
    [SerializeField]private Image visualKey;
    public void Init(string title, UnityAction onClickEvent, int index)
    {
        button.onClick.AddListener(onClickEvent);
        titleOfChoice.text = title;
        keyForChoice = allKeysForChoice[index];
        visualKey.sprite = visualKeysSprites[index];
        choiceButtonPressed = new UnityEvent();
        choiceButtonPressed.AddListener(onClickEvent);
    }

    public void Update()
    {
        if (keyForChoice.IsPressed() && choiceButtonPressed != null)
        {
            Debug.Log("Pressed");
            choiceButtonPressed.Invoke();
            choiceButtonPressed.RemoveAllListeners();
            choiceButtonPressed = null;
        }
    }
}
