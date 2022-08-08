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
            hasExtraProperties = target.GetType().GetFields().Any((f) =>
            f.Name != "actor_" &&
            f.Name != "actorDialogue" &&
            f.Name != "nodePosition" &&
            f.Name != "GUID"
            );
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            var nodedata = target as ANodeData;

            if (nodedata is ADialogueNodeData dialogueNodeData)
            {
                EditorGUILayout.BeginHorizontal();
                var textAreaStyle = new GUIStyle(EditorStyles.textArea);
                textAreaStyle.wordWrap = true;
                textAreaStyle.stretchHeight = true;
                dialogueNodeData.actorDialogue = EditorGUILayout.TextArea(dialogueNodeData.actorDialogue, textAreaStyle, GUILayout.MinWidth(240f), GUILayout.MaxWidth(260f), GUILayout.MinHeight(64f));
                EditorGUILayout.EndHorizontal();
            }

            if (hasExtraProperties)
            {
                EditorGUILayout.BeginVertical();
                foldoutState = EditorGUILayout.Foldout(foldoutState, "Extra");
                if (foldoutState)
                    DrawPropertiesExcluding(serializedObject, "actorDialogue", "actor_", "m_Script");
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.Space(1.4f);

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