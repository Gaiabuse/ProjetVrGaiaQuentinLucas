using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ChoiceButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text titleOfChoice;
    [SerializeField] private InputActionProperty[] keysForChoices;
    [SerializeField] private Sprite[] visualKeysSprites;
    [SerializeField] private Image visualKey;

    private UnityEvent choiceButtonPressed;
    private InputAction action;

    private void Awake()
    {
        choiceButtonPressed = null;
    }

    public void Init(string title, UnityAction onClickEvent, int index)
    {
        button.onClick.AddListener(onClickEvent);
        titleOfChoice.text = title;
        Debug.Log(index);
        Debug.Log(keysForChoices[index].action.name);
        action = keysForChoices[index].action;
        action.Enable();
        
        visualKey.sprite = visualKeysSprites[index];
        
        choiceButtonPressed = new UnityEvent();
        choiceButtonPressed.AddListener(onClickEvent);
    }

    private void Update()
    {
        if (action == null || !action.IsPressed() || choiceButtonPressed == null) return;
        choiceButtonPressed.Invoke();
        choiceButtonPressed.RemoveAllListeners();
        choiceButtonPressed = null;
    }
}