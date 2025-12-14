using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Scenario
{
    public class DialogueRunnerUI: MonoBehaviour
    {
        [SerializeField] private GameObject uiGameObject;
        [SerializeField] private TMP_Text speakerUI;
        [SerializeField] private TMP_Text dialogueUI;
        [SerializeField] private Transform choicesParent;
        [SerializeField] private ChoiceButton choicePrefab;
        [SerializeField] private float dialogueDurationByChar = 0.1f;
        public void SetUIGameObject(bool value)
        {
            uiGameObject.SetActive(value);
        }

        public void SetSpeaker(string name)
        {
            speakerUI.text = name;
        }

        public IEnumerator SetDialogue(string[] dialogueSplit)
        {
            foreach (var dialogueLine in dialogueSplit)
            {
                dialogueUI.text = dialogueLine;
                yield return new WaitForSeconds(dialogueDurationByChar*dialogueLine.Length);
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
            // tu aurais peut etre pu faire un systeme oû tu gardes les choix en cache, tu actives / désactives
            // et reset quand tu as besoin pour éviter la masse instanciation
            foreach (Transform children in choicesParent)
            {
                Destroy(children.gameObject);
            }
        }
    }
}
