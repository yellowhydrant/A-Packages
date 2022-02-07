//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UIElements;

//namespace A.Debugging
//{
//    public partial class AConsole : MonoBehaviour
//    {
//        VisualElement root;
//        VisualElement hierarchyContent;
//        AWindow hierarchyWindow;
//        VisualElement inspectorContent;

//        Vector3 offset = new Vector3(28, 0);
//        float maxWidth = 140;
//        float maxHeight = 16;

//        void Awake()
//        {
//            root = GetComponent<UIDocument>().rootVisualElement;
//            CreateGUI();
//        }

//        void CreateGUI()
//        {
//            hierarchyWindow = new AWindow();
//            hierarchyWindow.titleText.text = gameObject.scene.name;
//            hierarchyWindow.closeButton.clicked += () => root.Remove(hierarchyWindow);
//            hierarchyWindow.refreshButton.clicked += () => RefreshHierarchy();
//            hierarchyContent = hierarchyWindow.contentContainer;
//            root.Add(hierarchyWindow);         
//            RefreshHierarchy();

//            RefreshInspector();
//        }

//        void RefreshInspector()
//        {
//            //Call when clicked button
//            //Get the transform
//            //Get all of the other components
//            //Get all of the variables that aren't flagged with hidden and are one of the supported types
//            //Draw fields for these types
//            //Same goes for properties and serialize field
//            //Just draw everything thats editable in the editor
//            //Update the values every so often
//            //Also grab all of the methods that use parameters with supported types or just empty voids
//            //and add buttons for those unless marked hidden
//        }

//        void RefreshHierarchy()
//        {
//            hierarchyContent.Clear();
//            var roots = gameObject.scene.GetRootGameObjects();

//            var maxDepth = int.MaxValue;
//            var totalDepth = 0;
//            for (int i = 0; i < roots.Length; i++)
//            {
//                CreateButton(roots[i].name, roots[i].gameObject);
//                AddChildrenToHierarchy(roots[i].transform, 1, maxDepth, ref totalDepth);
//            }
//            var depth = Mathf.Min(maxDepth + (maxDepth < int.MaxValue ? 1 : 0), totalDepth);
//            hierarchyWindow.style.width = (depth - 1) * offset.x + maxWidth + 4;
//            hierarchyWindow.style.height = (maxHeight * 1.2f) * hierarchyContent.childCount + 18/*control bar height*/;
//        }

//        void AddChildrenToHierarchy(Transform parentTransform, int depth, int maxDepth, ref int totalDepth)
//        {
//            if (depth > totalDepth)
//                totalDepth = depth;
//            if (depth > maxDepth)
//                return;
//            for (int i = 0; i < parentTransform.childCount; i++)
//            {
//                var child = parentTransform.GetChild(i);
//                var button = CreateButton(child.name, child.gameObject);
//                button.transform.position = button.transform.position + (offset * depth);

//                AddChildrenToHierarchy(child, depth + 1, maxDepth, ref totalDepth);
//            }
//        }

//        AButton CreateButton(string name, GameObject gameObject)
//        {
//            var button = new AButton { text = name, gameObject = gameObject };
//            button.style.width = maxWidth;
//            button.style.height = maxHeight;
//            hierarchyContent.Add(button);
//            return button;
//        }

//        public class AButton : Button
//        {
//            public GameObject gameObject;
//        }
//    }
//}
