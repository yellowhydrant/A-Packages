using A;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AGUID), true)]
public class AGUIDEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GUI.enabled = false;
        DrawPropertiesExcluding(serializedObject, "m_Script");
        GUI.enabled = true;
    }
}
