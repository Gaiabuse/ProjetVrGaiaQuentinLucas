using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using XNode;
using XNodeEditor;

[CustomNodeEditor(typeof(DialogueNode))]
public class DialogueNodeEditor : NodeEditor
{
    private DialogueNode dialogueNode;
    public override void OnBodyGUI()
    {
        if(dialogueNode == null) dialogueNode = target as DialogueNode;
        serializedObject.Update();
        
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Title"));
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Text"));
        
        NodeEditorGUILayout.DynamicPortList("Responses" ,
            typeof(Response), 
            serializedObject,
            NodePort.IO.Output, 
            Node.ConnectionType.Multiple,
            Node.TypeConstraint.None,
            OnCreateReorderableList
            );
        
        serializedObject.ApplyModifiedProperties();
    }
    private void OnCreateReorderableList(ReorderableList list)
    {
        list.drawHeaderCallback = (Rect r) => EditorGUI.LabelField(r, "Responses (ports dynamiques)");

        list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var prop = serializedObject.FindProperty("Responses").GetArrayElementAtIndex(index);
            rect.y += 2;
            float w = rect.width;
            float thumb = 20f;

            EditorGUI.PropertyField(new Rect(rect.x + w * 0.56f, rect.y, w * 0.28f, EditorGUIUtility.singleLineHeight),
                prop.FindPropertyRelative("NextDialogue"),
                GUIContent.none);

            if (GUI.Button(new Rect(rect.x + w * 0.085f, rect.y, w * 0.15f - 4, EditorGUIUtility.singleLineHeight),
                    "Open"))
            {
                var obj = prop.FindPropertyRelative("NextDialogue").objectReferenceValue;
                if (obj != null)
                {
                    Selection.activeObject = obj;
                    EditorGUIUtility.PingObject(obj);
                }
                else
                {
                    var newData = ScriptableObject.CreateInstance<DialogueData>();
                    string path = "Assets/DialogueData_New.asset";
                    path = AssetDatabase.GenerateUniqueAssetPath(path);
                    AssetDatabase.CreateAsset(newData, path);
                    AssetDatabase.SaveAssets();
                    prop.FindPropertyRelative("nextDialogue").objectReferenceValue = newData;
                    serializedObject.ApplyModifiedProperties();
                    Selection.activeObject = newData;
                }
            }
        };
    }

    
}
