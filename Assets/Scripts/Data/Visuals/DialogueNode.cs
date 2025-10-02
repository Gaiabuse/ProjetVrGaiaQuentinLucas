using UnityEngine;
using XNode;

[CreateAssetMenu]
public class DialogueNode : Node
{
    [Input] public Node Previous;
    [Output(dynamicPortList = true)] public Node Next;
    
    
    public string Title;
    [TextArea(3, 6)] public string Text;

    [Output(dynamicPortList = true, connectionType = ConnectionType.Multiple)]
    public Response[] Responses;

    [HideInInspector] public Vector2 Size = new Vector2(300,150);
    public DialogueNode GetNextNode(int index)
    {
        var port = GetOutputPort("Responses" + index);
        if (port != null && port.Connection != null)
        {
            return port.Connection.node as DialogueNode;
        }
        return null;
    }
    
}
