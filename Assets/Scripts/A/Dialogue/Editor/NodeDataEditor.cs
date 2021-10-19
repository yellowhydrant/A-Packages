#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace A.Dialogue.Editor
{
    [CustomEditor(typeof(NodeData), true)]
    public class NodeDataEditor : UnityEditor.Editor
    {
        bool foldoutState;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            var nodedata = target as NodeData;

            EditorGUILayout.BeginHorizontal();
            nodedata.speakerSprite = (Sprite)EditorGUILayout.ObjectField(string.Empty, nodedata.speakerSprite, typeof(Sprite), false, GUILayout.MaxWidth(64f), GUILayout.MaxHeight(64f));
            var textAreaStyle = new GUIStyle(EditorStyles.textArea);
            textAreaStyle.wordWrap = true;
            textAreaStyle.stretchHeight = true;
            nodedata.speakerDialogue = EditorGUILayout.TextArea(nodedata.speakerDialogue, textAreaStyle, GUILayout.MaxWidth(320f-64f), GUILayout.MinHeight(64f), GUILayout.MaxHeight(64f));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(nodedata.styleTag)));
            foldoutState = EditorGUILayout.Foldout(foldoutState, "Extra");
            if (foldoutState)
            {
                DrawPropertiesExcluding(serializedObject, nameof(nodedata.speakerDialogue), nameof(nodedata.speakerName), nameof(nodedata.speakerSprite), nameof(nodedata.styleTag), "m_Script");
            }
            EditorGUILayout.EndVertical();

            //var fields = typeof(NodeData).GetFields();
            //fields = fields.Where((field) => field.GetCustomAttributes(typeof(HideInInspector), false).Length == 0).ToArray();

            //foreach (var field in fields)
            //{
            //    var property = serializedObject.FindProperty(field.Name);
            //    var labelStyle = new GUIStyle(EditorStyles.label);
            //    labelStyle.fontSize = 8;
            //    labelStyle.alignment = TextAnchor.MiddleCenter;
            //    EditorGUILayout.LabelField(new GUIContent(property.displayName), labelStyle);
            //    if (field.GetCustomAttributes(typeof(ATextAreaAttribute), false).Length > 0)
            //    {
            //        var textAreaStyle = new GUIStyle(EditorStyles.textArea);
            //        textAreaStyle.wordWrap = true;
            //        textAreaStyle.stretchHeight = true;
            //        property.stringValue = EditorGUILayout.TextArea(property.stringValue, textAreaStyle);
            //    }
            //    else
            //    {
            //        EditorGUILayout.PropertyField(property, new GUIContent(), true);
            //    }
            //}


            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }

        private static Texture2D TextureField(string name, Texture2D texture)
        {
            GUILayout.BeginVertical();
            var style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.UpperCenter;
            style.fixedWidth = 70;
            GUILayout.Label(name, style);
            var result = (Texture2D)EditorGUILayout.ObjectField(texture, typeof(Texture2D), false, GUILayout.Width(70), GUILayout.Height(70));
            GUILayout.EndVertical();
            return result;
        }
    }
}
#endif