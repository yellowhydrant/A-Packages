#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace A.Dialogue.Editor
{
    [CustomEditor(typeof(ANodeData), true)]
    public class ANodeDataEditor : UnityEditor.Editor
    {
        bool foldoutState;
        bool hasExtraProperties;

        private void OnEnable()
        {
            var nodedata = target as ANodeData;
            hasExtraProperties = typeof(ANodeData).GetFields().Any((f) =>
            f.Name != "speaker_" &&
            f.Name != "speakerDialogue" &&
            f.Name != "nodePosition" &&
            f.Name != "GUID" &&
            f.Name != "isBranch"
            );
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            var nodedata = target as ANodeData;

            EditorGUILayout.BeginHorizontal();
            var textAreaStyle = new GUIStyle(EditorStyles.textArea);
            textAreaStyle.wordWrap = true;
            textAreaStyle.stretchHeight = true;
            nodedata.speakerDialogue = EditorGUILayout.TextArea(nodedata.speakerDialogue, textAreaStyle, GUILayout.MinWidth(240f), GUILayout.MaxWidth(240f), GUILayout.MinHeight(64f));
            EditorGUILayout.EndHorizontal();

            if (hasExtraProperties)
            {
                EditorGUILayout.BeginVertical();
                foldoutState = EditorGUILayout.Foldout(foldoutState, "Extra");
                if (foldoutState)
                    DrawPropertiesExcluding(serializedObject, nameof(nodedata.speakerDialogue), "speaker_", "m_Script");
                EditorGUILayout.EndVertical();
            }
            else
            {
                EditorGUILayout.Space(1.4f);
            }

            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }

        //private static Texture2D TextureField(string name, Texture2D texture)
        //{
        //    GUILayout.BeginVertical();
        //    var style = new GUIStyle(GUI.skin.label);
        //    style.alignment = TextAnchor.UpperCenter;
        //    style.fixedWidth = 70;
        //    GUILayout.Label(name, style);
        //    var result = (Texture2D)EditorGUILayout.ObjectField(texture, typeof(Texture2D), false, GUILayout.Width(70), GUILayout.Height(70));
        //    GUILayout.EndVertical();
        //    return result;
        //}
    }
}
#endif