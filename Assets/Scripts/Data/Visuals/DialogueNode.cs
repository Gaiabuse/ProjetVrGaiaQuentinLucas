using UnityEngine;
using XNode;

public class DialogueNode : Node
{
    public string Title;
    [TextArea(3, 6)] public string Text;

    [Output(dynamicPortList = true, connectionType = ConnectionType.Multiple)]
    public Response[] Responses;

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
