using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;
namespace Kurisu.AkiBT.Editor
{
    public class GraphEditorWindow : EditorWindow
    {
        // GraphView window per UObject
        private static readonly Dictionary<int, GraphEditorWindow> cache = new();
        private BehaviorTreeView graphView;
        public BehaviorTreeView GraphView => graphView;
        private UnityEngine.Object Key { get; set; }
        protected virtual Type SOType => typeof(BehaviorTreeSO);
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
            var treeSO = CreateInstance(SOType);
            if (!graphView.Save())
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
            int code = Key.GetHashCode();
            if (Key != null && cache.ContainsKey(code))
            {
                if (Setting.AutoSave)
                {
                    if (!cache[code].graphView.Save())
                    {
                        var newWindow = cache[code] = Instantiate(this);
                        newWindow.rootVisualElement.Clear();
                        newWindow.rootVisualElement.Add(newWindow.CreateToolBar());
                        newWindow.graphView = graphView;
                        newWindow.rootVisualElement.Add(graphView);
                        graphView.EditorWindow = newWindow;
                        newWindow.Key = Key;
                        newWindow.Show();
                        newWindow.ShowNotification(new GUIContent("Auto save failed !"));
                        return;
                    }
                    Debug.Log($"<color=#3aff48>{graphView.TreeEditorName}</color>[{graphView.BehaviorTree._Object.name}] saved succeed ! {DateTime.Now}");
                }
                cache.Remove(code);
            }
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
        protected virtual void DrawLeftToolBar()
        {
            if (Application.isPlaying) return;

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
        }
        protected virtual void DrawRightToolBar()
        {
            if (Application.isPlaying) return;

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