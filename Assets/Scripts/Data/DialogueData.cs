using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "DialogueData", menuName = "Scenario/Dialogue")]
public class DialogueData : SerializedScriptableObject
{
    [BoxGroup("Meta")]
    [HideLabel]
    [InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)]
    public string ID;
    
    
    [BoxGroup("Meta")]
    [Title("Texte")]
    [TextArea(3, 8)]
    public string Text;
    
    [BoxGroup("Reponses")]
    [SerializeReference]
    [ListDrawerSettings(OnTitleBarGUI = "OnAddResponseButton")]
    public List<Response> Responses;
    
    private void OnAddResponseButton()
    {
        if (!GUILayout.Button("Add Response")) return;
        var r = new Response();
        Responses.Add(r);
#if UNITY_EDITOR
        ResponseEditorWindow.ShowWindow(r);
#endif
    }
    [BoxGroup("Conditions")]
    [SerializeReference]
    [ListDrawerSettings(Expanded = false)]
    public List<ConditionData> ConditionsForShow;
}
