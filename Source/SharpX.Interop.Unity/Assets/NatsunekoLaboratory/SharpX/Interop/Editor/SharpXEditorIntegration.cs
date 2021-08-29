using UnityEditor;
using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;

namespace NatsunekoLaboratory.SharpX.Interop
{
    public class SharpXEditorIntegration : EditorWindow
    {
        private const string StyleGuid = "f373bfe263071164d8d92de9bdbcd482";
        private const string XamlGuid = "daef8a6151b67de4297cba435d472518";
        public const string Path = "Assets/SharpX.asset";

        private SharpXConfiguration _configuration;
        private SerializedObject _so;

        [MenuItem("Window/Natsuneko Laboratory/SharpX Editor Integration")]
        public static void ShowWindow()
        {
            var window = GetWindow<SharpXEditorIntegration>();
            window.titleContent = new GUIContent("SharpX Editor Integration");
            window.minSize = new Vector2(400, 400);
            window.Show();
        }

        private static T LoadAssetByGuid<T>(string guid) where T : Object
        {
            return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));
        }

        // ReSharper disable once InconsistentNaming
        public void CreateGUI()
        {
            _configuration = AssetDatabase.LoadAssetAtPath<SharpXConfiguration>(Path);

            if (_configuration == null)
            {
                _configuration = CreateInstance<SharpXConfiguration>();
                AssetDatabase.CreateAsset(_configuration, Path);
                AssetDatabase.SaveAssets();
            }

            _so = new SerializedObject(_configuration);
            _so.Update();

            // Each editor window contains a root VisualElement object
            var root = rootVisualElement;
            root.styleSheets.Add(LoadAssetByGuid<StyleSheet>(StyleGuid));

            var xaml = LoadAssetByGuid<VisualTreeAsset>(XamlGuid);
            var tree = xaml.CloneTree();
            tree.Bind(_so);
            root.Add(tree);

            var button = root.Query<Button>("apply-button").First();
            button.clicked += OnClickApply;
        }

        private void OnClickApply()
        {
            _so.ApplyModifiedPropertiesWithoutUndo();
            AssetDatabase.SaveAssets();
        }
    }
}