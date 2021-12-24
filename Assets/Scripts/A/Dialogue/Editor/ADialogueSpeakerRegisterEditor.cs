#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace A.Dialogue.Editor
{
    [CustomEditor(typeof(ADialogueSpeakerRegister), true)]
    public class ADialogueSpeakerRegisterEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            var register = (ADialogueSpeakerRegister)target;
            var speakers = register.speakers;

            EditorGUILayout.BeginVertical();
            if (GUILayout.Button("Add Speaker"))
                register.AddSpeaker();
            EditorGUILayout.Separator();
            for (int i = 0; i < speakers.Count; i++)
            {
                ADialogueSpeakerEditor editor = (ADialogueSpeakerEditor)CreateEditor(speakers[i]);
                if (editor != null)
                {
                    editor.remove = register.RemoveSpeaker;
                    //else
                    //Remove null speaker
                    editor.OnInspectorGUI();
                }
                EditorGUILayout.Separator();
            }
            EditorGUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif