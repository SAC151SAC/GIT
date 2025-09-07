using UnityEngine;
using UnityEditor;

// 不使用命名空间，确保全局可访问
public class ReadOnly : PropertyAttribute { }

#if UNITY_EDITOR


[CustomPropertyDrawer(typeof(ReadOnly))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        bool wasEnabled = GUI.enabled;
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = wasEnabled;
    }
}
#endif


