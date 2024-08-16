#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Callbacks;
using A.Editor;

namespace A.Dialogue.Editor
{
    //TODO: Add select method and make selection like in behaviour tree
    //TODO: Add overlay
    //TODO: Add dialogue graph field to toolbar in script 
    public class ADialogueGraphEditor : EditorWindow
    {
        ADialogueGraphView graphView;

        ObjectField assetField;
        ACreateAssetOverlay overlay;

        ADialogueGraph graph;
        bool lockState;

        [MenuItem(AConstants.MenuItemRoot + "/Dialogue Graph Editor")]
        public static void OpenWindow()
        {
            ADialogueGraphEditor wnd = GetWindow<ADialogueGraphEditor>();
            wnd.titleContent = new GUIContent("Dialogue Graph Editor");
            wnd.minSize = new Vector2(800, 400);
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

            graphView = root.Q<ADialogueGraphView>();

            overlay = new ACreateAssetOverlay("Create New Dialogue Graph");
            overlay.createButton.clicked += () => CreateNewGraph(overlay.nameField.value);
            overlay.nameField.value = "New Dialogue Graph";
            root.Add(overlay);

            AAssetModificationProcessorCallbacks.OnWillDelete += (arg0, arg1) =>
            {
                if (AssetDatabase.LoadAssetAtPath<ADialogueGraph>(arg0) == graph)
                    SelectGraph(null);
                return AssetDeleteResult.DidNotDelete;
            };

            var toolbar = root.Q<Toolbar>();
            assetField = new ObjectField();
            assetField.objectType = typeof(ADialogueGraph);
            assetField.RegisterValueChangedCallback((ctx) => SelectGraph(ctx.newValue as ADialogueGraph));
            toolbar.Add(assetField);

            toolbar.Add(new ToolbarSpacer());

            var createButton = new Button();
            createButton.text = "Create New Dialogue";
            createButton.clicked += () => overlay.Show();
            toolbar.Add(createButton);

            var lockButton = new Button();
            lockButton.text = "Unlocked";
            lockButton.clicked += () =>
            {
                lockState = !lockState;
                lockButton.text = lockState ? "Locked" : "Unlocked";
            };
            toolbar.Add(lockButton);

            if (graph == null)
            {
                OnSelectionChange();
            }
            else
            {
                SelectGraph(graph);
            }
        }

        private void OnSelectionChange()
        {
            if (lockState)
                return;
            EditorApplication.delayCall += () => {
                ADialogueGraph graph = Selection.activeObject as ADialogueGraph;
                if (!graph)
                {
                    if (Selection.activeGameObject)
                    {
                        ADialogueParser parser = Selection.activeGameObject.GetComponent<ADialogueParser>();
                        if (parser)
                        {
                            if (parser.graph)
                                SelectGraph(parser.graph);
                        }
                    }
                }
                else
                {
                    SelectGraph(graph);
                }
            };
        }

        void SelectGraph(ADialogueGraph newGraph)
        {
            if (graphView == null)
                return;

            this.graph = newGraph;
            assetField.SetValueWithoutNotify(graph);

            overlay.Hide();

            graphView.PopulateView(graph);

            if (!graph)
                return;

            EditorApplication.delayCall += () => {
                graphView.FrameAll();
            };
        }

        void CreateNewGraph(string assetName)
        {
            string path = System.IO.Path.Combine(overlay.pathField.value, $"{assetName}.asset");
            ADialogueGraph graph = ScriptableObject.CreateInstance<ADialogueGraph>();
            graph.name = overlay.name.ToString();
            AssetDatabase.CreateAsset(graph, path);
            AssetDatabase.SaveAssets();
            Selection.activeObject = graph;
            EditorGUIUtility.PingObject(graph);
        }
    }
}
#endif