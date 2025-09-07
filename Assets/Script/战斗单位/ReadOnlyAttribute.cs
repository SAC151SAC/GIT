using UnityEngine;
using UnityEditor;

// ��ʹ�������ռ䣬ȷ��ȫ�ֿɷ���
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


