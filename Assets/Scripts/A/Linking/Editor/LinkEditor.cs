#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.SceneManagement;

namespace A.Linking.Editor
{
    public class DialogueLinkEditor : EditorWindow
    {
        LinkView view;

        [MenuItem("Tools/Link Editor")]
        public static void ShowExample()
        {
            DialogueLinkEditor wnd = GetWindow<DialogueLinkEditor>();
            wnd.titleContent = new GUIContent("LinkEditor");
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/A/Linking/Editor/LinkEditor.uxml");
            visualTree.CloneTree(root);

            // A stylesheet can be added to a VisualElement.
            // The style will be applied to the VisualElement and all of its children.
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/A/Linking/Editor/LinkEditor.uss");
            root.styleSheets.Add(styleSheet);

            view = root.Q<LinkView>();

            view.PopulateView();

            EditorSceneManager.activeSceneChangedInEditMode += (arg0, arg1) => view.PopulateView();
            EditorSceneManager.sceneDirtied += (arg) => view.PopulateView();
            EditorSceneManager.sceneSaved += (arg) => view.PopulateView();
        }
    }
}
#endif