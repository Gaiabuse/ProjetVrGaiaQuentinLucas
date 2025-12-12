using System.Collections;
using System.Collections.Generic;
using Exploration;
using Fight;
using Nodes;
using UnityEngine;
using XNode;

namespace Scenario
{
    public class DialogueRunner : MonoBehaviour
    {
        [SerializeField] private DialogueRunnerUI dialogueRunnerUI;
        [SerializeField] private Animator animatorManager;
        [SerializeField] private Vector3 positionForFight;
        private DialogueGraph _graph;
        private Vector3 _positionStartFight;
        private Coroutine _dialogueRunner;
        private Coroutine _choicesCoroutine;
        private Coroutine _fightCoroutine;
        private bool _asWin;
        private bool _fightEnded;
        private bool _switchNode;
        private Coroutine _dayTimer = null;
        private void OnEnable()
        {
            FightManager.FightEnded += b =>
            {
                _asWin = b;
                _fightEnded = true;
            };
            GameManager.ReturnToMainMenu += StopDayTimer;
        }

        private void OnDisable()
        {
            FightManager.FightEnded -= b =>
            {
                _asWin = b;
                _fightEnded = true;
            };
            GameManager.ReturnToMainMenu -= StopDayTimer;
        }

        private void StopDayTimer()
        {
            if (_dayTimer != null)
            {
                StopCoroutine(_dayTimer);
            }
        }
    
        public  void StartDialogue(DialogueGraph dialogueGraph)
        {
            _graph = dialogueGraph;
            dialogueRunnerUI.SetUIGameObject(true);
            PlayerManager.INSTANCE.SetCanMove(false);
            foreach (BaseNode node in _graph.nodes)
            {
                if (node.GetString() == "Start")
                {
                    _graph.Current = node;
                    break;
                }
            }
            _dialogueRunner = StartCoroutine(Runner());
        }

        private void EndDialogue()
        {
            _dayTimer = StartCoroutine(DayManager.INSTANCE.StartDayTimer());
            dialogueRunnerUI.SetUIGameObject(false);
            PlayerManager.INSTANCE.SetCanMove(true);
        }

        private void SetPosition(StartNode node)
        {
            if(node.SetPosition)
                PlayerManager.INSTANCE.TeleportPlayer(node.StartDialoguePosition);
        }
        private IEnumerator Runner()
        {
            Debug.Log("Running dialogue");
            bool waitEndOfAction = false;
            BaseNode currentNode = _graph.Current;
            string data = currentNode.GetString();
            string[] dataParts = data.Split('/');// dataParts[0] = type of the node
            // if is DialogueNode : dataParts[1] = speaker name, dataParts[2] = dialogue
            ChooseAction(dataParts,currentNode, out waitEndOfAction);

            if (waitEndOfAction)
            {
                yield return new WaitUntil(() => _switchNode);
            }
        }

        private void ChooseAction(string[] dataParts, Node currentNode,out bool waitEndOfAction)
        {
            switch (dataParts[0])
            {
                case "Start":
                    SetPosition(currentNode as StartNode);
                    NextNode("Exit");
                    waitEndOfAction = false;
                    break;
                case "Dialogue":
                    DialogueNode(dataParts, currentNode as DialogueNode);
                    waitEndOfAction = true;
                    break;
                case "Fight":
                    _fightCoroutine = StartCoroutine(FightNode(currentNode as FightNode));
                    waitEndOfAction = true;
                    break;
                case "End":
                    EndDialogue();
                    waitEndOfAction = false;
                    break;
            }

            waitEndOfAction = false;
        }
    
        private void DialogueNode(string[] dataParts, DialogueNode currentNode)
        {
            PlayerManager.INSTANCE.AddDialogueNode(currentNode);
            dialogueRunnerUI.SetSpeaker($"- {dataParts[1]} -");
            string[] dialogueSplit = DialogueSplit(dataParts[2]); 
            StartCoroutine(InstantiateDialogues(dialogueSplit,currentNode ));
        }
        private string[] DialogueSplit(string dialogue)
        {
            return  dialogue.Split('|');
        }
        private IEnumerator InstantiateDialogues(string[] dialogueSplit, DialogueNode dialogueNode)
        {
            Coroutine dialogueCoroutine = StartCoroutine(dialogueRunnerUI.SetDialogue(dialogueSplit));
            yield return dialogueCoroutine;
            _choicesCoroutine = StartCoroutine(InstantiateChoices(dialogueNode));
        }
    
        private IEnumerator InstantiateChoices(DialogueNode node)
        {
            if (node.Choices.Length == 1)
            {
                yield return new WaitForSeconds(2.5f);
                NextNode("Choices " +0);
            }
            else
            {
                List<ChoiceButton> choices = dialogueRunnerUI.InstantiateChoices(node.Choices.Length);
                for(int i  = 0; i < choices.Count; i++)
                {
                    var i1 = i;
                    choices[i].Init(node.Choices[i], () =>
                    {
                        NextNode("Choices " + i1);
                        animatorManager.SetTrigger("Choices"+i1);
                    },i);
                }
            }
        }
    
        private IEnumerator FightNode(FightNode node)
        {
            SetFightNode(node);
            yield return new WaitUntil(()=>_fightEnded);
            EndFightNode();
        }

        private void SetFightNode(FightNode node)
        {
            _fightEnded = false;
            _positionStartFight = PlayerManager.INSTANCE.GetPlayerPosition();
            PlayerManager.INSTANCE.TeleportPlayer(positionForFight);
            dialogueRunnerUI.SetUIGameObject(false);
            if (node != null) FightManager.INSTANCE.StartFight(node.Level);
        }

        private void EndFightNode()
        {
            PlayerManager.INSTANCE.TeleportPlayer(positionForFight);
            dialogueRunnerUI.SetUIGameObject(true);
            NextNode(_asWin ? "AsWin" : "AsLoose");
        }
    
        private void NextNode(string fieldName)
        {
            _switchNode = false;
            _switchNode = true;
            KillAllCoroutines();
            foreach (NodePort port in _graph.Current.Ports)
            {
                if (port.fieldName == fieldName)
                {
                    if (!port.IsConnected) continue;
                    _graph.Current = port.Connection.node as BaseNode;
                    break;
                }
            }
            dialogueRunnerUI.KillChoices();
            _switchNode = false;
            _dialogueRunner = StartCoroutine(Runner());
        }
        private void KillAllCoroutines()
        {
            if (_dialogueRunner != null)
            {
                StopCoroutine(_dialogueRunner);
                _dialogueRunner = null;
            }

            if (_choicesCoroutine != null)
            {
                StopCoroutine(_choicesCoroutine);
                _choicesCoroutine = null;
            }

            if (_fightCoroutine != null)
            {
                StopCoroutine(_fightCoroutine);
                _fightCoroutine = null;
            }
        }
    }
}
