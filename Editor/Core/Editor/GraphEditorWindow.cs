using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;
using UnityEditor.Callbacks;
using System.Reflection;
namespace Kurisu.AkiBT.Editor
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DefaultAssetTypeAttribute : Attribute
    {
        public Type AssetType { get; }
        public DefaultAssetTypeAttribute(Type type)
        {
            AssetType = type;
        }
    }
    [DefaultAssetType(typeof(BehaviorTreeAsset))]
    public class GraphEditorWindow : EditorWindow, IHasCustomMenu
    {
        // GraphView window per UObject
        private static readonly Dictionary<int, GraphEditorWindow> cache = new();
        private BehaviorTreeView graphView;
        public BehaviorTreeView GraphView => graphView;
        private UnityEngine.Object Key { get; set; }
        protected virtual string TreeName => "Behavior Tree";
        protected virtual string InfoText => "Welcome to AkiBT, an ultra-simple behavior tree!";
        private static BehaviorTreeSetting setting;
        private static BehaviorTreeSetting Setting
        {
            get
            {
                if (setting == null) setting = BehaviorTreeSetting.GetOrCreateSettings();
                return setting;
            }
        }
        public Type GetAssetType()
        {
            var attribute = GetType().GetCustomAttribute<DefaultAssetTypeAttribute>();
            return attribute?.AssetType ?? typeof(BehaviorTreeAsset);
        }
#pragma warning disable IDE0051
        [OnOpenAsset]
        private static bool OnOpenAsset(int instanceId, int _)
        {
            if (cache.ContainsKey(instanceId))
            {
                cache[instanceId].Show();
                cache[instanceId].Focus();
                return true;
            }
            var asset = EditorUtility.InstanceIDToObject(instanceId);
            if (asset.GetType() == typeof(BehaviorTreeAsset))
            {
                Show((IBehaviorTreeContainer)asset);
                return true;
            }
            if (asset.GetType().IsSubclassOf(typeof(BehaviorTreeAsset)))
            {
                var windowTypes = SubclassSearchUtility.FindSubClassTypes(typeof(GraphEditorWindow));
                foreach (var windowType in windowTypes)
                {
                    var attribute = windowType.GetCustomAttribute<DefaultAssetTypeAttribute>();
                    if (attribute != null && attribute.AssetType == asset.GetType())
                    {
                        var bt = (IBehaviorTreeContainer)asset;
                        var window = CreateInstance(windowType) as GraphEditorWindow;
                        window.RepaintGraphView(bt);
                        window.titleContent = new GUIContent($"{window.graphView.EditorName} ({bt.Object.name})");
                        window.Key = bt.Object;
                        cache[instanceId] = window;
                        window.Show();
                        window.Focus();
                        return true;
                    }
                }
            }
            return false;
        }
#pragma warning restore IDE0051
        [MenuItem("Tools/AkiBT/AkiBT Editor")]
        private static void ShowEditorWindow()
        {
            string path = EditorUtility.SaveFilePanel("Select ScriptableObject save path", Application.dataPath, "BehaviorTreeSO", "asset");
            if (string.IsNullOrEmpty(path)) return;
            path = path.Replace(Application.dataPath, string.Empty);
            var treeSO = CreateInstance<BehaviorTreeAsset>();
            AssetDatabase.CreateAsset(treeSO, $"Assets/{path}");
            AssetDatabase.SaveAssets();
            Show(treeSO);
        }
        public static bool ContainsEditorWindow(int windowInstanceID)
        {
            foreach (var value in cache.Values)
            {
                if (value.GetInstanceID() == windowInstanceID) return true;
            }
            return false;
        }
        public static void Show(IBehaviorTreeContainer bt)
        {
            var window = Create<GraphEditorWindow>(bt);
            window.Show();
            window.Focus();
        }
        /// <summary>
        /// Instantiate new GraphEditorWindow
        /// </summary>
        /// <param name="bt"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <returns></returns>
        public static T Create<T>(IBehaviorTreeContainer bt) where T : GraphEditorWindow
        {
            var key = bt.Object.GetHashCode();
            if (cache.ContainsKey(key))
            {
                return (T)cache[key];
            }
            var window = CreateInstance<T>();
            window.RepaintGraphView(bt);
            window.titleContent = new GUIContent($"{window.graphView.EditorName} ({bt.Object.name})");
            window.Key = bt.Object;
            cache[key] = window;
            return window;
        }
        private void RepaintGraphView(IBehaviorTreeContainer bt)
        {
            rootVisualElement.Clear();
            rootVisualElement.Add(CreateToolBar());
            graphView = StructGraphView(bt);
            rootVisualElement.Add(graphView);
        }
        protected virtual BehaviorTreeView StructGraphView(IBehaviorTreeContainer bt)
        {
            var graphView = new BehaviorTreeView(bt, this);
            var infoView = new InfoView(InfoText);
            infoView.styleSheets.Add(Resources.Load<StyleSheet>("AkiBT/Info"));
            graphView.Add(infoView);
            graphView.OnNodeSelect = (node) => infoView.UpdateSelection(node);
            graphView.Restore();
            return graphView;
        }
        private void SaveToAsset(string path)
        {
            var treeSO = CreateInstance(GetAssetType());
            if (!graphView.Validate())
            {
                Debug.LogWarning($"<color=#ff2f2f>{graphView.EditorName}</color>: Save failed, asset wasn't created!\n{DateTime.Now}");
                return;
            }
            graphView.Commit((IBehaviorTreeContainer)treeSO);
            string savePath = $"{path}/{Key.name}.asset";
            AssetDatabase.CreateAsset(treeSO, savePath);
            AssetDatabase.SaveAssets();
            Debug.Log($"<color=#3aff48>{graphView.EditorName}</color>: Save succeed, asset created path: {savePath}\n{DateTime.Now}");
        }

        private void OnDestroy()
        {
            if (Key == null) return;
            int code = Key.GetHashCode();
            if (Key != null && cache.TryGetValue(code, out var editor))
            {
                if (Setting.AutoSave && !Application.isPlaying)
                {
                    if (!editor.graphView.Save())
                    {
                        var msg = "Auto save failed, do you want to discard change?";
                        if (EditorUtility.DisplayDialog("Warning", msg, "Cancel", "Discard"))
                        {
                            var newWindow = cache[code] = Clone();
                            newWindow.Show();
                        }
                        else
                        {
                            cache.Remove(code);
                        }
                        return;
                    }
                    Debug.Log($"<color=#3aff48>{graphView.EditorName}</color>[{graphView.Container.Object.name}] saved succeed! {DateTime.Now}");
                }
                cache.Remove(code);
            }
        }
        /// <summary>
        /// Clone the editor window for preventing data loss when reloading or quitting
        /// </summary>
        /// <returns></returns>
        protected virtual GraphEditorWindow Clone()
        {
            var newWindow = Instantiate(this);
            newWindow.rootVisualElement.Clear();
            newWindow.rootVisualElement.Add(newWindow.CreateToolBar());
            newWindow.graphView = graphView;
            newWindow.rootVisualElement.Add(graphView);
            graphView.EditorWindow = newWindow;
            newWindow.Key = Key;
            return newWindow;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange playModeStateChange)
        {
            switch (playModeStateChange)
            {
                case PlayModeStateChange.EnteredEditMode:
                    Reload();
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    Reload();
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(playModeStateChange), playModeStateChange, null);
            }
        }

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            Reload();
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }
        void IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Reload"), false, Reload);
        }
        private void Reload()
        {
            if (Key != null)
            {
                if (Key is GameObject) RepaintGraphView((Key as GameObject).GetComponent<IBehaviorTreeContainer>());
                else RepaintGraphView(Key as IBehaviorTreeContainer);
                Repaint();
            }
        }
        private VisualElement CreateToolBar()
        {
            return new IMGUIContainer(
                () =>
                {
                    GUILayout.BeginHorizontal(EditorStyles.toolbar);
                    DrawLeftToolBar();
                    GUILayout.FlexibleSpace();
                    DrawRightToolBar();
                    GUILayout.EndHorizontal();
                }
            );
        }
        /// <summary>
        /// Override to customize left toolbar
        /// </summary>
        protected virtual void DrawLeftToolBar()
        {
            GUI.enabled = !Application.isPlaying;
            if (GUILayout.Button($"Save {TreeName}", EditorStyles.toolbarButton))
            {
                var guiContent = new GUIContent();
                if (graphView.Save())
                {
                    guiContent.text = $"Update {TreeName} Succeed!";
                    ShowNotification(guiContent);
                }
                else
                {
                    guiContent.text = $"Invalid {TreeName}, please check the node connection for errors!";
                    ShowNotification(guiContent);
                }
            }
            GUI.enabled = true;
            bool newValue = GUILayout.Toggle(Setting.AutoSave, "Auto Save", EditorStyles.toolbarButton);
            if (newValue != Setting.AutoSave)
            {
                Setting.AutoSave = newValue;
                EditorUtility.SetDirty(setting);
                AssetDatabase.SaveAssets();
            }
            if (GUILayout.Button("Save to Asset", EditorStyles.toolbarButton))
            {
                string path = EditorUtility.OpenFolderPanel("Select asset save path", Setting.LastPath, "");
                if (!string.IsNullOrEmpty(path))
                {
                    Setting.LastPath = path;
                    SaveToAsset(BehaviorTreeEditorUtility.GetRelativePath(path));
                }

            }
            GUI.enabled = !Application.isPlaying;
            if (GUILayout.Button("Copy from Asset", EditorStyles.toolbarButton))
            {
                string path = EditorUtility.OpenFilePanel("Select asset to copy", Setting.LastPath, "asset");
                var data = LoadDataFromFile(BehaviorTreeEditorUtility.GetRelativePath(path));
                if (data != null)
                {
                    Setting.LastPath = path;
                    EditorUtility.SetDirty(setting);
                    AssetDatabase.SaveAssets();
                    ShowNotification(new GUIContent("Asset Dropped Succeed!"));
                    graphView.CopyFromTree(data.GetBehaviorTree(), new Vector3(400, 300));
                }
            }
            GUI.enabled = true;
        }
        /// <summary>
        /// Override to customize right toolbar
        /// </summary>
        protected virtual void DrawRightToolBar()
        {
            if (GUILayout.Button("Auto Layout", EditorStyles.toolbarButton))
            {
                NodeAutoLayoutHelper.Layout(new BehaviorTreeLayoutConvertor().Init(graphView));
            }
            if (GUILayout.Button("Save to Json", EditorStyles.toolbarButton))
            {
                var serializedData = graphView.SerializeTreeToJson();
                string path = EditorUtility.SaveFilePanel("Select json file save path", Setting.LastPath, graphView.Container.Object.name, "json");
                if (!string.IsNullOrEmpty(path))
                {
                    FileInfo info = new(path);
                    Setting.LastPath = info.Directory.FullName;
                    EditorUtility.SetDirty(setting);
                    File.WriteAllText(path, serializedData);
                    Debug.Log($"<color=#3aff48>{GraphView.EditorName}</color>: Save to json file succeed!");
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
            GUI.enabled = !Application.isPlaying;
            if (GUILayout.Button("Copy from Json", EditorStyles.toolbarButton))
            {
                string path = EditorUtility.OpenFilePanel("Select json file to copy", Setting.LastPath, "json");
                if (!string.IsNullOrEmpty(path))
                {
                    FileInfo info = new(path);
                    Setting.LastPath = info.Directory.FullName;
                    EditorUtility.SetDirty(setting);
                    AssetDatabase.SaveAssets();
                    var data = File.ReadAllText(path);
                    if (graphView.CopyFromJson(data, new Vector3(400, 300)))
                        ShowNotification(new GUIContent("Json file read succeed!"));
                    else
                        ShowNotification(new GUIContent("Json file is in wrong format!"));
                }
            }
            GUI.enabled = true;
        }
        protected IBehaviorTreeContainer LoadDataFromFile(string path)
        {
            try
            {
                return AssetDatabase.LoadAssetAtPath<BehaviorTreeAsset>(path);

            }
            catch
            {
                ShowNotification(new GUIContent($"Invalid Path: {path}, please pick ScriptableObject inherited from BehaviorTreeSO"));
                return null;
            }
        }
    }
}