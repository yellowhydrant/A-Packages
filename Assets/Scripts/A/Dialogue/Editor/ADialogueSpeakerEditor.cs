#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace A.Dialogue.Editor
{
    [CustomEditor(typeof(ADialogueActor), true)]
    public class ADialogueSpeakerEditor : UnityEditor.Editor
    {
        public System.Action<ADialogueActor> remove;
        public override void OnInspectorGUI()
        {
            if (target == null || serializedObject == null)
                return;
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            var speaker = (ADialogueActor)target;

            EditorGUILayout.BeginHorizontal();
            speaker.sprite = (Sprite)EditorGUILayout.ObjectField(string.Empty, speaker.sprite, typeof(Sprite), false, GUILayout.MaxWidth(64f), GUILayout.MaxHeight(64f));
            speaker.name = EditorGUILayout.TextField(speaker.name);
            if (GUILayout.Button("X", EditorStyles.miniButton))
                remove.Invoke(speaker);
            EditorGUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif