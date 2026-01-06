using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scenario
{
    public class DialogueRunnerUI: MonoBehaviour
    {
        [SerializeField] private DialogueRunner runner;
        [SerializeField] private GameObject uiGameObject;
        [SerializeField] private TMP_Text speakerUI;
        [SerializeField] private TMP_Text dialogueUI;
        [SerializeField] private Transform choicesParent;
        [SerializeField] private ChoiceButton choicePrefab;
        [SerializeField] private float dialogueDurationByChar = 0.1f;
        [SerializeField] private InputActionProperty switchToNextDialogue;
        private InputAction _action;

        private void Start()
        {
            _action = switchToNextDialogue.action;
            _action.Enable();
        }

        public void SetUIGameObject(bool value)
        {
            uiGameObject.SetActive(value);
        }

        public void SetSpeaker(string name)
        {
            speakerUI.text = name;
        }

        public IEnumerator SetDialogue(string[] dialogueSplit,AudioClip[] voices, CharacterAnimator currentSpeaker)
        {
            for (int i = 0; i < dialogueSplit.Length; i++)
            {
                runner.StopVoices();
                dialogueUI.text = dialogueSplit[i];
                if (currentSpeaker != null)
                {
                    StartCoroutine(runner.PlayVoices(voices[i],currentSpeaker)); 
                }
                yield return new WaitUntil(_action.IsPressed);
            }
            
        }

        public List<ChoiceButton> InstantiateChoices(int choicesCount)
        {
            List<ChoiceButton> choices = new List<ChoiceButton>();
            for (int i = 0; i < choicesCount; i++)
            {
                ChoiceButton choice = Instantiate(choicePrefab, choicesParent);
                choices.Add(choice);
            }
            return choices;
        }

        public void KillChoices()
        {
            // tu aurais peut-etre pu faire un systeme oû tu gardes les choix en cache, tu actives / désactives
            // et reset quand tu as besoin pour éviter la masse instanciation
            foreach (Transform children in choicesParent)
            {
                Destroy(children.gameObject);
            }
        }
    }
}
