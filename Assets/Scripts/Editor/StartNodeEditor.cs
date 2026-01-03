using Nodes;
using XNodeEditor;

#if UNITY_EDITOR
namespace Editor
{
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

			NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Characters"));
			
			serializedObject.ApplyModifiedProperties();
		}
	}
}
#endif