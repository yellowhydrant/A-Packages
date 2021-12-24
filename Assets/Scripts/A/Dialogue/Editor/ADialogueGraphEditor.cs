#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Callbacks;
using A;

namespace A.Dialogue.Editor
{
    public class ADialogueGraphEditor : EditorWindow
    {
        ADialogueGraphView view;

        TextField pathField;
        TextField nameField;
        ObjectField assetField;

        public ADialogueGraph dialogueGraph;

        [MenuItem(AConstants.ItemMenuRoot + "/Dialogue Graph Editor")]
        public static void OpenWindow()
        {
            ADialogueGraphEditor wnd = GetWindow<ADialogueGraphEditor>();
            wnd.titleContent = new GUIContent("Dialogue Graph Editor");
            wnd.minSize = new Vector2(800, 600);
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            if (Selection.activeObject is ADialogueGraph)
            {
                OpenWindow();
                return true;
            }
            return false;
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ADialogueConstants.VisualTreeAssetPath);
            visualTree.CloneTree(root);

            // Link stylesheet
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(ADialogueConstants.StyleSheetPath);
            root.styleSheets.Add(styleSheet);

            view = root.Q<ADialogueGraphView>();

            var newBut = root.Q<Button>("create-new-graph");
            newBut.clicked += () =>
            {
                var asset = AssetDatabase.LoadAssetAtPath<ADialogueGraph>(pathField.value + nameField.value + ".asset");

                if (string.IsNullOrEmpty(nameField.value))
                    return;

                if (asset != null)
                {
                    Debug.LogWarning("The asset you're trying to create already exists. The exsiting asset has been opened instead!");
                    dialogueGraph = asset;
                    return;
                }

                asset = ScriptableObject.CreateInstance<ADialogueGraph>();
                AssetDatabase.CreateAsset(asset, pathField.value + nameField.value + ".asset");
                AssetDatabase.SaveAssets();
                dialogueGraph = asset;
            };

            pathField = root.Q<TextField>("asset-path-folder");
            nameField = root.Q<TextField>("asset-path-name");

            assetField = root.Q<ObjectField>("asset-field");

            assetField.RegisterValueChangedCallback((c) =>
            {
                dialogueGraph = c.newValue as ADialogueGraph;
                view.PopulateView(dialogueGraph);
            });

            assetField.bindingPath = nameof(dialogueGraph);
            assetField.Bind(new SerializedObject(this));

            dialogueGraph = Selection.activeObject as ADialogueGraph;
            Selection.activeObject = null;
        }

        private void OnInspectorUpdate()
        {
            if (assetField != null && assetField.value == null && !view.isClear)
                assetField.value = null;
        }

        
    }
}
#endif