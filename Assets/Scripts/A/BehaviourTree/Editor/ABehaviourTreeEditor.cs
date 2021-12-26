using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Callbacks;
using A.Editor;

namespace A.BehaviourTree
{
    //TODO: Add the description field to the node itself (follow TheKiwiCoder Tut)
    public class ABehaviourTreeEditor : EditorWindow {

        ABehaviourTreeView treeView;
        ABehaviourTree tree;
        InspectorView inspectorView;
        IMGUIContainer blackboardView;
        Toolbar toolBar;
        ObjectField assetField;
        ACreateAssetOverlay overlay;

        SerializedObject treeObject;
        SerializedProperty blackboardProperty;

        [MenuItem(AConstants.MenuItemRoot + "/Behaviour Tree")]
        public static void OpenWindow() {
            ABehaviourTreeEditor wnd = GetWindow<ABehaviourTreeEditor>();
            wnd.titleContent = new GUIContent("BehaviourTreeEditor");
            wnd.minSize = new Vector2(800, 600);
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line) {
            if (Selection.activeObject is ABehaviourTree) {
                OpenWindow();
                return true;
            }
            return false;
        }

        List<T> LoadAssets<T>() where T : UnityEngine.Object {
            string[] assetIds = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            List<T> assets = new List<T>();
            foreach (var assetId in assetIds) {
                string path = AssetDatabase.GUIDToAssetPath(assetId);
                T asset = AssetDatabase.LoadAssetAtPath<T>(path);
                assets.Add(asset);
            }
            return assets;
        }

        public void CreateGUI() {

            //var settings = BehaviourTreeSettings.GetOrCreateSettings();

            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ABehaviourTreeConstants.TreeVisualTreeAssetPath);
            visualTree.CloneTree(root);

            // A stylesheet can be added to a VisualElement.
            // The style will be applied to the VisualElement and all of its children.
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(ABehaviourTreeConstants.TreeStyleSheetPath);// settings.behaviourTreeStyle;
            root.styleSheets.Add(styleSheet);

            // Main treeview
            treeView = root.Q<ABehaviourTreeView>();
            treeView.OnNodeSelected = OnNodeSelectionChanged;

            // Inspector View
            inspectorView = root.Q<InspectorView>();

            // Blackboard view
            blackboardView = root.Q<IMGUIContainer>();
            blackboardView.onGUIHandler = () => {
                if (treeObject != null && treeObject.targetObject != null) {
                    treeObject.Update();
                    EditorGUILayout.PropertyField(blackboardProperty);
                    treeObject.ApplyModifiedProperties();
                }
            };

            // Toolbar assets menu
            toolBar = root.Q<Toolbar>();
            assetField = new ObjectField();
            assetField.objectType = typeof(ABehaviourTree);
            assetField.RegisterValueChangedCallback((ctx) => { SelectTree(ctx.newValue as ABehaviourTree); });
            toolBar.Add(assetField);
            toolBar.Add(new ToolbarSpacer());
            var createNewButton = new Button();
            createNewButton.text = "New Behaviour Tree";
            createNewButton.clicked += () => overlay.Show();
            toolBar.Add(createNewButton);
            //var behaviourTrees = LoadAssets<ABehaviourTree>();
            //behaviourTrees.ForEach(tree => {
            //    toolbarMenu.menu.AppendAction($"{tree.name}", (a) => {
            //        Selection.activeObject = tree;
            //    });
            //});
            //toolbarMenu.menu.AppendSeparator();
            //toolbarMenu.menu.AppendAction("New Tree...", (a) => CreateNewTree("NewBehaviourTree"));

            // New Tree Dialog
            overlay = new ACreateAssetOverlay("Create New Tree");
            root.Add(overlay);
            overlay.createButton.clicked += () => CreateNewTree(overlay.nameField.value);
            AAssetModificationProcessorCallbacks.OnWillDelete += (arg0, arg1) =>
            {
                if (AssetDatabase.LoadAssetAtPath<ABehaviourTree>(arg0) != null)
                    SelectTree(null);
                return AssetDeleteResult.DidNotDelete; };

            if (tree == null) {
                OnSelectionChange();
            } else {
                SelectTree(tree);
            }
        }

        private void OnEnable() {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnDisable() {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange obj) {
            switch (obj) {
                case PlayModeStateChange.EnteredEditMode:
                    OnSelectionChange();
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    OnSelectionChange();
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    break;
            }
        }

        private void OnSelectionChange() {
            EditorApplication.delayCall += () => {
                ABehaviourTree tree = Selection.activeObject as ABehaviourTree;
                if (!tree) {
                    if (Selection.activeGameObject) {
                        ABehaviourTreeRunner runner = Selection.activeGameObject.GetComponent<ABehaviourTreeRunner>();
                        if (runner) {
                            tree = runner.tree;
                        }
                    }
                }

                SelectTree(tree);
            };
        }

        void SelectTree(ABehaviourTree newTree) {

            if (treeView == null) {
                return;
            }

            //if (!newTree) {
            //    return;
            //}

            this.tree = newTree;
            assetField.SetValueWithoutNotify(tree);

            overlay.Hide();

            if (Application.isPlaying) {
                treeView.PopulateView(tree);
            } else {
                treeView.PopulateView(tree);
            }

            if (!tree)
                return;

            treeObject = new SerializedObject(tree);
            blackboardProperty = treeObject.FindProperty(nameof(tree.blackboard));

            EditorApplication.delayCall += () => {
                treeView.FrameAll();
            };
        }

        void OnNodeSelectionChanged(ANodeView node) {
            inspectorView.UpdateSelection(node);
        }

        private void OnInspectorUpdate() {
            treeView?.UpdateNodeStates();
        }

        void CreateNewTree(string assetName) {
            string path = System.IO.Path.Combine(overlay.pathField.value, $"{assetName}.asset");
            ABehaviourTree tree = ScriptableObject.CreateInstance<ABehaviourTree>();
            tree.name = overlay.name.ToString();
            AssetDatabase.CreateAsset(tree, path);
            AssetDatabase.SaveAssets();
            Selection.activeObject = tree;
            EditorGUIUtility.PingObject(tree);
        }
    }
}