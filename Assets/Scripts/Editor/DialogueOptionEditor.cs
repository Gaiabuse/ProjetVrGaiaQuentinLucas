#if UNITY_EDITOR
using Scenario;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(DialogueOption))]
    public class DialogueOptionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty dialogue = property.FindPropertyRelative("dialogue");
            SerializedProperty isFallBack = property.FindPropertyRelative("isFallback");
            SerializedProperty conditions = property.FindPropertyRelative("conditions");
        
            Rect line = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            EditorGUI.PropertyField(line, dialogue);
            line.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            EditorGUI.PropertyField(line, isFallBack);
            line.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            if (!isFallBack.boolValue)
            {
                EditorGUI.PropertyField(line, conditions, true);
            }

            EditorGUI.EndProperty();
        }
    
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = 0f;

            SerializedProperty dialogue = property.FindPropertyRelative("dialogue");
            SerializedProperty isFallBack = property.FindPropertyRelative("isFallback");
            SerializedProperty conditions = property.FindPropertyRelative("conditions");
        
            height += EditorGUI.GetPropertyHeight(dialogue) + EditorGUIUtility.standardVerticalSpacing;
        
            height += EditorGUI.GetPropertyHeight(isFallBack) + EditorGUIUtility.standardVerticalSpacing;

            if (!isFallBack.boolValue)
            {
                height += EditorGUI.GetPropertyHeight(conditions, true) + EditorGUIUtility.standardVerticalSpacing;
            }

            return height;
        }
    }
}
#endif
