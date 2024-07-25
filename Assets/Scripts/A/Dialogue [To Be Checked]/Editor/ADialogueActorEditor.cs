#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace A.Dialogue.Editor
{
    [CustomEditor(typeof(ADialogueActor), true)]
    public class ADialogueActorEditor : UnityEditor.Editor
    {
        public System.Action<ADialogueActor> remove;
        public override void OnInspectorGUI()
        {
            if (target == null || serializedObject == null)
                return;
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            var actor = (ADialogueActor)target;

            EditorGUILayout.BeginHorizontal();
            actor.sprite = (Sprite)EditorGUILayout.ObjectField(string.Empty, actor.sprite, typeof(Sprite), false, GUILayout.MaxWidth(64f), GUILayout.MaxHeight(64f));
            DrawPropertiesExcluding(serializedObject, "m_Script", nameof(actor.guid), nameof(actor.sprite));
            //actor.name = EditorGUILayout.TextField(actor.name);
            //if (GUILayout.Button("X", EditorStyles.miniButton))
            //    remove.Invoke(actor);
            EditorGUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif