#if UNITY_EDITOR
using A.UI;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(AButton), true)]
[CanEditMultipleObjects]
public class AButtonEditor : ButtonEditor
{
    List<SerializedProperty> propertiesToDraw = new List<SerializedProperty>();

    protected override void OnEnable()
    {
        base.OnEnable();
        var fields = target.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (var field in fields)
        {
            if ((field.IsPublic && !field.GetCustomAttributes<HideInInspector>().Any()) || (field.IsPrivate && field.GetCustomAttributes<SerializeField>().Any()))
            {
                propertiesToDraw.Add(serializedObject.FindProperty(field.Name));
            }
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        EditorGUI.BeginChangeCheck();
        for (int i = 0; i < propertiesToDraw.Count; i++)
        {
            EditorGUILayout.PropertyField(propertiesToDraw[i]);
        }
        if (EditorGUI.EndChangeCheck())
            serializedObject.ApplyModifiedProperties();
    }
}
#endif
