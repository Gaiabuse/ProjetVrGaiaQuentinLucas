using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Scenario
{
    public class ChoiceButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text titleOfChoice;
        [SerializeField] private InputActionProperty[] inputKeysForChoices;
        [SerializeField] private Sprite[] spritesOfInputKeys;
        [SerializeField] private Image visualOfInput;

        private UnityEvent _choiceButtonPressed;
        private InputAction _action;

        private void Awake()
        {
            _choiceButtonPressed = null;
        }

        public void Init(string title, UnityAction onClickEvent, int index)
        {
            button.onClick.AddListener(onClickEvent);
            titleOfChoice.text = title;
            Debug.Log(index);
            Debug.Log(inputKeysForChoices[index].action.name);
            _action = inputKeysForChoices[index].action;
            _action.Enable();
        
            visualOfInput.sprite = spritesOfInputKeys[index];
        
            _choiceButtonPressed = new UnityEvent();
            _choiceButtonPressed.AddListener(onClickEvent);
        }

        private void Update()
        {
            if (_action == null || !_action.IsPressed() || _choiceButtonPressed == null) return;
            _choiceButtonPressed.Invoke();
            _choiceButtonPressed.RemoveAllListeners();
            _choiceButtonPressed = null;
        }
    }
}