#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.SceneManagement;

namespace A.Linking.Editor
{
    public class ALinkEditor : EditorWindow
    {
        ALinkView view;

        [MenuItem(AConstants.MenuItemRoot + "/Link Editor")]
        public static void ShowExample()
        {
            ALinkEditor wnd = GetWindow<ALinkEditor>();
            wnd.titleContent = new GUIContent("LinkEditor");
            wnd.minSize = new Vector2(800, 600);
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ALinkingConstants.VisualTreeAssetPath);
            visualTree.CloneTree(root);

            // A stylesheet can be added to a VisualElement.
            // The style will be applied to the VisualElement and all of its children.
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(ALinkingConstants.StyleSheetPath);
            root.styleSheets.Add(styleSheet);

            view = root.Q<ALinkView>();

            view.PopulateView();

            EditorSceneManager.activeSceneChangedInEditMode += (arg0, arg1) => view.PopulateView();
            EditorSceneManager.sceneDirtied += (arg) => view.PopulateView();
            EditorSceneManager.sceneSaved += (arg) => view.PopulateView();
        }
    }
}
#endif