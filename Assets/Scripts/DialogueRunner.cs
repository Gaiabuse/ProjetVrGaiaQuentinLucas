using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using XNode;

public class DialogueRunner : MonoBehaviour
{
    [SerializeField] private DialogueGraph graph;
    [SerializeField] private TMP_Text speaker;
    [SerializeField] private TMP_Text dialogue;
    [SerializeField] private Transform choicesParent;
    [SerializeField] private ChoiceButton choicePrefab;
    Coroutine dialogueRunner;
    private Coroutine choicesCoroutine;
    private bool switchNode;
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
        dialogueRunner = StartCoroutine(Runner());
    }


    private IEnumerator Runner()
    {
        BaseNode currentNode = graph.current;
        string data = currentNode.GetString();
        string[] dataParts = data.Split('/');
        if (dataParts[0] == "Start")
        {
            NextNode("Exit");
        }
        if (dataParts[0] == "DialogueNode")
        {
            speaker.text = dataParts[1];
            dialogue.text = dataParts[2];
            choicesCoroutine = StartCoroutine(InstantiateChoices(currentNode as DialogueNode));
            yield return new WaitUntil(() => switchNode );
        }
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
                choice.Init(node.Choices[i], () => NextNode("Choices "+i1));
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
                    Debug.Log(port.Connection.node.name);
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
