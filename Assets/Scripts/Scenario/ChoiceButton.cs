using TMPro;
using Unity.Mathematics;
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
        [SerializeField] private Image fill;
        [SerializeField] private float maxHoldTime = 2f;
        private UnityEvent _choiceButtonPressed;
        private InputAction _action;

        private float _holdTime;
        private bool _isHolding;
        private void Awake()
        {
            _choiceButtonPressed = null;
        }

        public void Init(string title, UnityAction onClickEvent, int index)
        {
            button.onClick.AddListener(onClickEvent);
            titleOfChoice.text = title;
            _action = inputKeysForChoices[index].action;
            _action.Enable();
        
            visualOfInput.sprite = spritesOfInputKeys[index];
        
            _choiceButtonPressed = new UnityEvent();
            _choiceButtonPressed.AddListener(onClickEvent);
            InitAction();
        }

        private void InitAction()
        {
            _action.started += StartHold;
            _action.canceled += RevertHold;
            _action.performed += FinishHold;
        }

        private void FinishHold(InputAction.CallbackContext context)
        {
            _choiceButtonPressed?.Invoke();
            _isHolding = false;
            GameManager.INSTANCE.ChoiceSelected = false;
            ClearAction();
        }

        private void StartHold(InputAction.CallbackContext context)
        {
            if(GameManager.INSTANCE.ChoiceSelected)return;
            GameManager.INSTANCE.ChoiceSelected = true;
            _holdTime = 0f;
            _isHolding = true;
        }

        private void RevertHold(InputAction.CallbackContext context)
        {
           _isHolding = false;
           GameManager.INSTANCE.ChoiceSelected = false;
           fill.fillAmount = 0f;
        }
        private void ClearAction()
        {
            _action.started -= StartHold;
            _action.canceled -= RevertHold;
            _action.performed -= FinishHold;
        }

        private void Update()
        {
            if (_action == null || !_isHolding|| _choiceButtonPressed == null) return;
            _holdTime += Time.deltaTime;
            float holdTimeNormalized = math.clamp(_holdTime / maxHoldTime, 0f, 1f);
            fill.fillAmount = holdTimeNormalized;
        }
        
    }
}