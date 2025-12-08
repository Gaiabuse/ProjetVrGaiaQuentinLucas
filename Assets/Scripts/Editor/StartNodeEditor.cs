using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using XNodeEditor;
#if UNITY_EDITOR
[CustomNodeEditor(typeof(StartNode))]
public class StartNodeEditor : NodeEditor
{
	public override void OnBodyGUI()
	{
		serializedObject.Update();

		StartNode node = target as StartNode;
		
		NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Exit"));
		
		NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("SetPosition"));
		
		if (node.SetPosition)
		{
			NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("StartDialoguePosition"));
		}
		serializedObject.ApplyModifiedProperties();
	}
}
#endif