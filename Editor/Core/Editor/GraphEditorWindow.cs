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
    [DefaultAssetType(typeof(BehaviorTreeSO))]
    public class GraphEditorWindow : EditorWindow, IHasCustomMenu
    {
        // GraphView window per UObject
        private static readonly Dictionary<int, GraphEditorWindow> cache = new();
        private BehaviorTreeView graphView;
        public BehaviorTreeView GraphView => graphView;
        private UnityEngine.Object Key { get; set; }
        protected virtual string TreeName => "Behavior Tree";
        protected virtual string InfoText => "Welcome to AkiBT, an ultra-simple behavior tree !";
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
            return attribute?.AssetType ?? typeof(BehaviorTreeSO);
        }
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
            if (asset.GetType() == typeof(BehaviorTreeSO))
            {
                Show((IBehaviorTree)asset);
                return true;
            }
            if (asset.GetType().IsSubclassOf(typeof(BehaviorTreeSO)))
            {
                var windowTypes = SubclassSearchUtility.FindSubClassTypes(typeof(GraphEditorWindow));
                foreach (var windowType in windowTypes)
                {
                    var attribute = windowType.GetCustomAttribute<DefaultAssetTypeAttribute>();
                    if (attribute != null && attribute.AssetType == asset.GetType())
                    {
                        var bt = (IBehaviorTree)asset;
                        var window = CreateInstance(windowType) as GraphEditorWindow;
                        window.RepaintGraphView(bt);
                        window.titleContent = new GUIContent($"{window.graphView.TreeEditorName} ({bt._Object.name})");
                        window.Key = bt._Object;
                        cache[instanceId] = window;
                        window.Show();
                        window.Focus();
                        return true;
                    }
                }
            }
            return false;
        }
        [MenuItem("Tools/AkiBT/AkiBT Editor")]
        private static void ShowEditorWindow()
        {
            string path = EditorUtility.SaveFilePanel("Select ScriptableObject save path", Application.dataPath, "BehaviorTreeSO", "asset");
            if (string.IsNullOrEmpty(path)) return;
            path = path.Replace(Application.dataPath, string.Empty);
            var treeSO = CreateInstance<BehaviorTreeSO>();
            AssetDatabase.CreateAsset(treeSO, $"Assets/{path}");
            AssetDatabase.SaveAssets();
            Show(treeSO);
        }
        public static void Show(IBehaviorTree bt)
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
        public static T Create<T>(IBehaviorTree bt) where T : GraphEditorWindow
        {
            var key = bt._Object.GetHashCode();
            if (cache.ContainsKey(key))
            {
                return (T)cache[key];
            }
            var window = CreateInstance<T>();
            window.RepaintGraphView(bt);
            window.titleContent = new GUIContent($"{window.graphView.TreeEditorName} ({bt._Object.name})");
            window.Key = bt._Object;
            cache[key] = window;
            return window;
        }
        private void RepaintGraphView(IBehaviorTree bt)
        {
            rootVisualElement.Clear();
            rootVisualElement.Add(CreateToolBar());
            graphView = StructGraphView(bt);
            rootVisualElement.Add(graphView);
        }
        protected virtual BehaviorTreeView StructGraphView(IBehaviorTree bt)
        {
            var graphView = new BehaviorTreeView(bt, this);
            var infoView = new InfoView(InfoText);
            infoView.styleSheets.Add(Resources.Load<StyleSheet>("AkiBT/Info"));
            graphView.Add(infoView);
            graphView.OnNodeSelect = (node) => infoView.UpdateSelection(node);
            graphView.Restore();
            return graphView;
        }
        private void SaveToSO(string path)
        {
            var treeSO = CreateInstance(GetAssetType());
            if (!graphView.Validate())
            {
                Debug.LogWarning($"<color=#ff2f2f>{graphView.TreeEditorName}</color> : Save failed, ScriptableObject wasn't created !\n{DateTime.Now}");
                return;
            }
            graphView.Commit((IBehaviorTree)treeSO);
            string savePath = $"Assets/{path}/{Key.name}.asset";
            AssetDatabase.CreateAsset(treeSO, savePath);
            AssetDatabase.SaveAssets();
            Debug.Log($"<color=#3aff48>{graphView.TreeEditorName}</color> : Save succeed, ScriptableObject created path : {savePath}\n{DateTime.Now}");
        }

        private void OnDestroy()
        {
            int code;
            if (Key != null && cache.ContainsKey(code = Key.GetHashCode()))
            {
                if (Setting.AutoSave && !Application.isPlaying)
                {
                    if (!cache[code].graphView.Save())
                    {
                        var msg = "Auto save failed, do you want to discard change ?";
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
                    Debug.Log($"<color=#3aff48>{graphView.TreeEditorName}</color>[{graphView.BehaviorTree._Object.name}] saved succeed ! {DateTime.Now}");
                }
                cache.Remove(code);
            }
        }
        /// <summary>
        /// Clone the editor window for preventing data lossing when reloading or quitting
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
                if (Key is GameObject) RepaintGraphView((Key as GameObject).GetComponent<IBehaviorTree>());
                else RepaintGraphView(Key as IBehaviorTree);
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
            if (GUILayout.Button("Save To SO", EditorStyles.toolbarButton))
            {
                string path = EditorUtility.OpenFolderPanel("Select ScriptableObject save path", Setting.LastPath, "");
                if (!string.IsNullOrEmpty(path))
                {
                    Setting.LastPath = path;
                    SaveToSO(path.Replace(Application.dataPath, string.Empty));
                }

            }
            GUI.enabled = !Application.isPlaying;
            if (GUILayout.Button("Copy From SO", EditorStyles.toolbarButton))
            {
                string path = EditorUtility.OpenFilePanel("Select ScriptableObject to copy", Setting.LastPath, "asset");
                var data = LoadDataFromFile(path.Replace(Application.dataPath, string.Empty));
                if (data != null)
                {
                    Setting.LastPath = path;
                    EditorUtility.SetDirty(setting);
                    AssetDatabase.SaveAssets();
                    ShowNotification(new GUIContent("Data Dropped Succeed!"));
                    graphView.CopyFromTree(data, new Vector3(400, 300));
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
            if (GUILayout.Button("Save To Json", EditorStyles.toolbarButton))
            {
                var serializedData = graphView.SerializeTreeToJson();
                string path = EditorUtility.SaveFilePanel("Select json file save path", Setting.LastPath, graphView.BehaviorTree._Object.name, "json");
                if (!string.IsNullOrEmpty(path))
                {
                    FileInfo info = new(path);
                    Setting.LastPath = info.Directory.FullName;
                    EditorUtility.SetDirty(setting);
                    File.WriteAllText(path, serializedData);
                    Debug.Log($"<color=#3aff48>{GraphView.TreeEditorName}</color>:Save json file succeed!");
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
            GUI.enabled = !Application.isPlaying;
            if (GUILayout.Button("Copy From Json", EditorStyles.toolbarButton))
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
                        ShowNotification(new GUIContent("Json file Read Succeed!"));
                    else
                        ShowNotification(new GUIContent("Json file is in wrong format!"));
                }
            }
            GUI.enabled = true;
        }
        protected IBehaviorTree LoadDataFromFile(string path)
        {
            try
            {
                return AssetDatabase.LoadAssetAtPath<BehaviorTreeSO>($"Assets/{path}");

            }
            catch
            {
                ShowNotification(new GUIContent($"Invalid Path:Assets/{path}, please pick ScriptableObject inherited from BehaviorTreeSO"));
                return null;
            }
        }
    }
}