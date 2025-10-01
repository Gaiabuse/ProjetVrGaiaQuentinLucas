using System;
using UnityEngine;

public class DialogueRunner : MonoBehaviour
{
    [SerializeField] private DialogueGraph graphAsset;
    [SerializeField] DialogueNode startNode;

    private DialogueNode currentNode;

    private void Start()
    {
        currentNode = startNode;
        PlayCurrent();
    }

    public void PlayCurrent( )
    {
        if(currentNode == null) return;
        Debug.Log(">>> " + currentNode.Text);
        for (int i = 0; i < currentNode.Responses.Length; i++)
        {
            Debug.Log($"[{i}] {currentNode.Responses[i]}");
        }
    }

    public void ChooseResponse( int responsesIndex)
    {
        if(currentNode == null) return;
       DialogueNode next = currentNode.GetNextNode(responsesIndex);
       if (next != null)
       {
           currentNode = next;
           PlayCurrent();
       }
       else
       {
           Debug.Log("End !");
       }
    }
}
