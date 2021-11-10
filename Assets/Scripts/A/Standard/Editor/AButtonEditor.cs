using A.UI;
using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(AButton), true)]
[CanEditMultipleObjects]
public class AButtonEditor : ButtonEditor
{
    SerializedProperty text;

    protected override void OnEnable()
    {
        base.OnEnable();
        text = serializedObject.FindProperty(nameof(AButton.text));
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(text);
        if (EditorGUI.EndChangeCheck())
            serializedObject.ApplyModifiedProperties();
    }
}
