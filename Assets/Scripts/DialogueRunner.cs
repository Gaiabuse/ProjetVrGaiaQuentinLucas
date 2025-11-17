using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using XNode;

public class DialogueRunner : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private GameObject Ui;
    [SerializeField] private DialogueGraph graph;
    [SerializeField] private TMP_Text speaker;
    [SerializeField] private TMP_Text dialogue;
    [SerializeField] private Transform choicesParent;
    [SerializeField] private ChoiceButton choicePrefab;
    [SerializeField] private float dialogueDurationByChar = 0.1f;
    [SerializeField] private Animator animatorManager;
    [SerializeField] private GameObject[] objectsForMove;
    [SerializeField] private Vector3 positionForFight;
    private Vector3 positionStartFight;
    private Coroutine dialogueRunner;
    private Coroutine choicesCoroutine;
    private bool switchNode;
    private bool asWin;
    private bool fightEnded;
    private void Start()
    {
        switchNode = false;
        foreach (BaseNode node in graph.nodes)
        {
            if (node.GetString() == "Start")
            {
                graph.current = node;
                break;
            }
        }
        StartDialogue();
    }

    private void OnEnable()
    {
        FightManager.INSTANCE.FightEnded += b =>
        {
            asWin = b;
            fightEnded = true;
        };
    }

    private void OnDisable()
    {
        FightManager.INSTANCE.FightEnded -= b =>
        {
            asWin = b;
            fightEnded = true;
        };
    }

    private void StartDialogue()
    {
        Ui.SetActive(true);
        foreach (GameObject obj in objectsForMove)
        {
            obj.SetActive(false);
        }
        dialogueRunner = StartCoroutine(Runner());
    }

    private void EndDialogue()
    {
        Ui.SetActive(false);
        foreach (GameObject obj in objectsForMove)
        {
            obj.SetActive(true);
        }
    }
    private IEnumerator Runner()
    {
        BaseNode currentNode = graph.current;
        string data = currentNode.GetString();
        string[] dataParts = data.Split('/');
        string[] dialogueSplit;
        switch (dataParts[0])
        {
            case "Start":
                NextNode("Exit");
                break;
            case "Dialogue":
                speaker.text = $"- {dataParts[1]} -";
                dialogueSplit = dataParts[2].Split('|');
                StartCoroutine(InstantiateDialogues(dialogueSplit,currentNode as DialogueNode));
                yield return new WaitUntil(() => switchNode );
                break;
            case "Fight":
                FightNode node = currentNode as FightNode;
                StartCoroutine(StartFightNode(node));
                yield return new WaitUntil(() => switchNode );
                break;
            case "End":
                EndDialogue();
                break;
            default: break;
        }
    }

    private IEnumerator StartFightNode(FightNode node)
    {
        fightEnded = false;
        positionStartFight = player.position;
        player.position = positionStartFight;
        Ui.SetActive(false);
        if (node != null) FightManager.INSTANCE.StartFight(node.level);
        yield return new WaitUntil(()=>fightEnded);
        player.position = positionStartFight;
        Ui.SetActive(true);
        NextNode(asWin ? node.AsWin : node.AsLose);
    }

    private IEnumerator InstantiateDialogues(string[] dialogueSplit, DialogueNode dialogueNode)
    {
        foreach (var t in dialogueSplit)
        {
            dialogue.text = t;
            yield return new WaitForSeconds(dialogueDurationByChar*t.Length);
        }
        choicesCoroutine = StartCoroutine(InstantiateChoices(dialogueNode));
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
            for(int i  = 0; i < node.Choices.Length; i++)
            {
                ChoiceButton choice =  Instantiate(choicePrefab, choicesParent);
                var i1 = i;
                choice.Init(node.Choices[i], () =>
                {
                    NextNode("Choices " + i1);
                    animatorManager.SetTrigger("Choices"+i1);
                });
            }
        }
    }
    
    private void NextNode(string fieldName)
    {
        switchNode = true;
        switchNode = false;
        if (dialogueRunner != null)
        {
            StopCoroutine(dialogueRunner);
            dialogueRunner = null;
        }

        if (choicesCoroutine != null)
        {
            StopCoroutine(choicesCoroutine);
            choicesCoroutine = null;
        }
        foreach (NodePort port in graph.current.Ports)
        {
            if (port.fieldName == fieldName)
            {
                if (port.IsConnected)
                {
                    graph.current = port.Connection.node as BaseNode;
                    break;
                }
            }
        }

        foreach (Transform children in choicesParent)
        {
            Destroy(children.gameObject);
        }
        
        dialogueRunner = StartCoroutine(Runner());
    }
  
}
