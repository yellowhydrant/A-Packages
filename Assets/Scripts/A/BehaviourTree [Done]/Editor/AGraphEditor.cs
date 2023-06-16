#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using A.Editor;

namespace A.BehaviourTree
{
    public abstract class AGraphEditor<TEditor, TAsset> : EditorWindow where TEditor : EditorWindow where TAsset : ScriptableObject
    {
        public static string windowTitle;
        public static string assetName;

        protected Toolbar toolBar;
        protected ObjectField assetField;
        protected ACreateAssetOverlay overlay;

        protected TAsset activeAsset;

        public void CreateGUI(out VisualElement root, out StyleSheet styleSheet)
        {
            // Each editor window contains a root VisualElement object
            root = rootVisualElement;

            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ABehaviourTreeConstants.TreeVisualTreeAssetPath);
            visualTree.CloneTree(root);

            // A stylesheet can be added to a VisualElement.
            // The style will be applied to the VisualElement and all of its children.
            styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(ABehaviourTreeConstants.TreeStyleSheetPath);// settings.behaviourTreeStyle;
            root.styleSheets.Add(styleSheet);


            // Toolbar assets menu
            toolBar = root.Q<Toolbar>();
            assetField = new ObjectField();
            assetField.objectType = typeof(TAsset);
            assetField.RegisterValueChangedCallback((ctx) => { SelectAsset(ctx.newValue as TAsset); });
            toolBar.Add(assetField);

            toolBar.Add(new ToolbarSpacer());

            var createNewButton = new Button();
            createNewButton.text = $"New {assetName}";
            createNewButton.clicked += () => overlay.Show();
            toolBar.Add(createNewButton);

            //lock part


            // New Tree Dialog
            overlay = new ACreateAssetOverlay($"Create New {assetName}");
            overlay.createButton.clicked += () => CreateNewAsset(overlay.nameField.value);
            overlay.nameField.value = $"New {assetName}";
            root.Add(overlay);

            AAssetModificationProcessorCallbacks.OnWillDelete += (arg0, arg1) =>
            {
                if (AssetDatabase.LoadAssetAtPath<ABehaviourTree>(arg0) == activeAsset)
                    SelectAsset(null);
                return AssetDeleteResult.DidNotDelete;
            };

            if (activeAsset == null)
            {
                OnSelectionChange();
            }
            else
            {
                SelectAsset(activeAsset);
            }
        }

        protected void OnSelectionChange()
        {
            EditorApplication.delayCall += () => {
                TAsset asset = Selection.activeObject as TAsset;
                if (!asset)
                {
                    OnSelectionChangeFail();
                }
                SelectAsset(asset);
            };
        }

        public abstract void OnSelectionChangeFail();

        public abstract void SelectAsset(TAsset asset);

        public abstract void CreateNewAsset(string assetName);
    }
}


#endif