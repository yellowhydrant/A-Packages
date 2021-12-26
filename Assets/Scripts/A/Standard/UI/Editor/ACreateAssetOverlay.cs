#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace A.Editor
{
    public class ACreateAssetOverlay : VisualElement
    {
        public Label titleText;
        public TextField nameField;
        public TextField pathField;
        public Button createButton;
        public Button selectButton;
        public Button closeButton;
        public VisualElement overlay;

        public new class UxmlFactory : UxmlFactory<ACreateAssetOverlay, VisualElement.UxmlTraits>
        {

        }

        public ACreateAssetOverlay()
        {
            Ctor();
        }

        public ACreateAssetOverlay(string title)
        {
            Ctor();
            titleText.text = title;
        }

        void Ctor()
        {
            style.position = Position.Absolute;
            style.left = 0;
            style.right = 0;
            style.top = 0;
            style.bottom = 0;
            style.alignItems = Align.Center;
            style.alignContent = Align.Center;
            style.backgroundColor = new Color(0, 0, 0, .5f);
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/A/Standard/UI/Editor/Overlay.uxml");//Change to non hardcoded path
            visualTree.CloneTree(this);
            titleText = this.Q<Label>("TitleText");
            titleText.text = "Create New";
            nameField = this.Q<TextField>("Name");
            pathField = this.Q<TextField>("Path");
            pathField.RegisterValueChangedCallback((ctx) =>
            {
                pathField.SetValueWithoutNotify(ctx.newValue.Substring(ctx.newValue.IndexOf("Assets")));
            });
            overlay = this.Q<VisualElement>("Overlay");
            createButton = this.Q<Button>("CreateButton");
            selectButton = this.Q<Button>("SelectButton");
            selectButton.clicked += () =>
            {
                pathField.value = EditorUtility.OpenFolderPanel(titleText.text, "Assets/", "New Folder");
            };
            closeButton = this.Q<Button>("CloseButton");
            closeButton.clicked += () => Hide();
        }

        public void Hide() => style.visibility = Visibility.Hidden;
        public void Show() => style.visibility = Visibility.Visible;
    }
}
#endif
