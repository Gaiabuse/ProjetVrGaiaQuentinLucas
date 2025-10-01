using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
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
    public DialogueData NextDialogue;

    [ShowIf("IsGotoDialogue")]
    [Button("Create new dialogue")]
    private void CreateNewDialogue()
    {
#if UNITY_EDITOR
        var newDialogue = ScriptableObject.CreateInstance<DialogueData>();
        newDialogue.ID ="d_" + Guid.NewGuid().ToString("N").Substring(0, 6);
        newDialogue.Text ="Nouveau dialogue ...";
        
        string path = "Assets/Data/Scenario/Dialogues/Dialogue_" + newDialogue.ID + ".asset";
        AssetDatabase.CreateAsset(newDialogue, path);
        AssetDatabase.SaveAssets();
        
        NextDialogue = newDialogue;
#endif
    }

    [ShowIf("IsSetFight")] 
    public string FightId;
    
    private bool IsGotoDialogue() => Type == ResultType.newDialogue;
    private bool IsSetFight() => Type == ResultType.fight;
    
    public ConditionData[] ConditionsForShow;
}

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
