using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
#endif

using UnityEngine;

[Serializable]
public class Response 
{
    #region Enum

    public enum ResultType
    {
        newDialogue,
        fight,
        end
    }

    #endregion
    
    [LabelText("Responses Text")]
    [HideLabel]
    [MultiLineProperty(3)]
    public string Text;

    [EnumToggleButtons] 
    public ResultType Type;
    
    [ShowIf("IsGotoDialogue")]
    [InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)]
    [AssetsOnly]
    public DialogueNode NextDialogue;

    [ShowIf("IsSetFight")] 
    public string FightId;
    
    private bool IsGotoDialogue() => Type == ResultType.newDialogue;
    private bool IsSetFight() => Type == ResultType.fight;
    
    public ConditionData[] ConditionsForShow;
}

#if UNITY_EDITOR
public class ResponseEditorWindow : OdinEditorWindow
{
    private Response response;

    public static void ShowWindow(Response pResponse)
    {
        
        var window = CreateInstance<ResponseEditorWindow>();
        window.response = pResponse;
        window.titleContent = new GUIContent("Responses Editor");
        window.ShowUtility();
    }
    
    [InlineEditor(InlineEditorObjectFieldModes.Hidden)]
    [HideLabel]
    [ShowInInspector]
    private Response editableResponse => response;
}
#endif
