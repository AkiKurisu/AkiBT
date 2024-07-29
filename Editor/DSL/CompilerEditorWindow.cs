using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Kurisu.AkiBT.Editor;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
namespace Kurisu.AkiBT.DSL.Editor
{
    public class CompilerEditorWindow : EditorWindow
    {
        private class CompilerSetting
        {
            public bool enableMask;
            public string editorName = "AkiBT";
            public string inputCode = string.Empty;
            public string registryName = "NodeTypeRegistry";
        }
        private string InputCode { get => setting.inputCode; set => setting.inputCode = value; }
        private BehaviorTreeAsset _outputTree;
        private Vector2 m_ScrollPosition;
        private bool EnableMask { get => setting.enableMask; set => setting.enableMask = value; }
        private string EditorName { get => setting.editorName; set => setting.editorName = value; }
        private string RegistryNameName { get => setting.registryName; set => setting.registryName = value; }
        private static string KeyName => Application.productName + "_AkiBT_DSL_CompilerSetting";
        private CompilerSetting setting;
        public delegate Vector2 BeginVerticalScrollViewFunc(Vector2 scrollPosition, bool alwaysShowVertical, GUIStyle verticalScrollbar, GUIStyle background, params GUILayoutOption[] options);
        static BeginVerticalScrollViewFunc s_func;
        static BeginVerticalScrollViewFunc BeginVerticalScrollView
        {
            get
            {
                if (s_func == null)
                {
                    var methods = typeof(EditorGUILayout).GetMethods(BindingFlags.Static | BindingFlags.NonPublic).Where(x => x.Name == "BeginVerticalScrollView").ToArray();
                    var method = methods.First(x => x.GetParameters()[1].ParameterType == typeof(bool));
                    s_func = (BeginVerticalScrollViewFunc)method.CreateDelegate(typeof(BeginVerticalScrollViewFunc));
                }
                return s_func;
            }
        }
        private GUIStyle textAreaStyle;
        private GUIStyle labelStyle;
        private int state = 2;
        private int mTab;
        [MenuItem("Tools/AkiBT/DSL/Compiler")]
        public static void OpenEditor()
        {
            var window = GetWindow<CompilerEditorWindow>("AkiBT Compiler");
            window.maxSize = new Vector2(600, 800);
        }
        private void OnEnable()
        {
            var data = EditorPrefs.GetString(KeyName);
            setting = JsonConvert.DeserializeObject<CompilerSetting>(data);
            setting ??= new CompilerSetting();
        }
        private void OnDisable()
        {
            EditorPrefs.SetString(KeyName, JsonConvert.SerializeObject(setting));
        }
        private void GatherStyle()
        {
            textAreaStyle = new GUIStyle(GUI.skin.textArea) { wordWrap = true };
            labelStyle = new GUIStyle(GUI.skin.label) { richText = true };
        }
        private void OnGUI()
        {
            GatherStyle();
            int newTab = GUILayout.Toolbar(mTab, new string[] { "Compile", "Decompile" });
            if (newTab != mTab)
            {
                mTab = newTab;
                state = 2;
            }
            m_ScrollPosition = BeginVerticalScrollView(m_ScrollPosition, false, GUI.skin.verticalScrollbar, "OL Box");
            GUILayout.Label("Input Code");
            InputCode = EditorGUILayout.TextArea(InputCode, textAreaStyle, GUILayout.MaxHeight(700));
            EditorGUILayout.EndScrollView();
            GUILayout.FlexibleSpace();
            switch (mTab)
            {
                case 0:
                    DrawCompile();
                    break;
                case 1:
                    DrawDecompile();
                    break;
            }
            DrawToolbar();
        }
        private void DrawCompile()
        {
            GUILayout.Label("Output Tree");
            GUI.enabled = false;
            EditorGUILayout.ObjectField(_outputTree, typeof(BehaviorTreeComponent), false);
            GUI.enabled = true;
            var orgColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(140 / 255f, 160 / 255f, 250 / 255f);
            if (GUILayout.Button("Compile", GUILayout.MinHeight(25)))
            {
                EditorApplication.update += DelayCall;
            }
            GUI.backgroundColor = new Color(253 / 255f, 163 / 255f, 255 / 255f);
            if (state == 1 && GUILayout.Button("Export", GUILayout.MinHeight(25)))
            {
                string path = EditorUtility.OpenFolderPanel("Select saving path", Application.dataPath, "");
                if (string.IsNullOrEmpty(path)) return;
                var savePath = path.Replace(Application.dataPath, string.Empty);
                string outPutPath = $"Assets/{savePath}/ConvertTreeSO.asset";
                AssetDatabase.CreateAsset(_outputTree, outPutPath);
                AssetDatabase.SaveAssets();
                Log($"BehaviorTreeSO saved succeed! File Path:{outPutPath}");
                GUIUtility.ExitGUI();
            }
            GUI.backgroundColor = orgColor;
            DrawCompileResult(state);
        }
        private void DelayCall()
        {
            EditorApplication.update -= DelayCall;
            state = 0;
            state = Compile();
        }
        private void DrawDecompile()
        {
            DrawDragAndDrop();
            DrawDecompileResult(state);
            var orgColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(140 / 255f, 160 / 255f, 250 / 255f);
            if (GUILayout.Button("Decompile All", GUILayout.MinHeight(25)))
            {
                string path = EditorUtility.OpenFolderPanel("Choose decompile files saving path", Application.dataPath, "");
                if (string.IsNullOrEmpty(path)) return;
                //Using AkiBT Service
                var serviceData = BehaviorTreeSetting.GetOrCreateSettings().ServiceData;
                serviceData.ForceSetUp();
                var decompiler = new Decompiler();
                foreach (var pair in serviceData.serializationCollection.serializationPairs)
                {
                    if (pair.behaviorTreeAsset != null)
                    {
                        string data;
                        try
                        {
                            data = decompiler.Decompile(pair.behaviorTreeAsset);
                        }
                        catch
                        {
                            LogError($"Decompile failed with {pair.behaviorTreeAsset.name}");
                            continue;
                        }
                        string folderPath = path + $"/{pair.behaviorTreeAsset.GetType().Name}";
                        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
                        string savePath = $"{folderPath}/{pair.behaviorTreeAsset.name}_DSL.json";
                        File.WriteAllText(savePath, data);
                    }
                }
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                GUIUtility.ExitGUI();
            }
            GUI.backgroundColor = orgColor;
        }
        private void DrawDragAndDrop()
        {

            GUIStyle StyleBox = new(GUI.skin.box)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Italic,
                fontSize = 12
            };
            GUI.skin.box = StyleBox;
            Rect myRect = GUILayoutUtility.GetRect(0, 50, GUILayout.ExpandWidth(true));
            GUI.Box(myRect, "Drag and Drop BehaviorTree to this Box!", StyleBox);
            {
                if (Event.current.type == EventType.DragUpdated)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    Event.current.Use();

                }
                if (Event.current.type == EventType.DragPerform)
                {
                    if (DragAndDrop.objectReferences.Length > 0)
                    {
                        state = 0;
                        if (DragAndDrop.objectReferences[0] is GameObject gameObject)
                            state = Decompile(gameObject.GetComponent<IBehaviorTreeContainer>());
                        else if (DragAndDrop.objectReferences[0] is IBehaviorTreeContainer behaviorTree)
                            state = Decompile(behaviorTree);
                        else
                            state = -1;
                    }
                }
            }
        }
        private void DrawToolbar()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Registry Name", labelStyle);
            RegistryNameName = GUILayout.TextField(RegistryNameName, GUILayout.MinWidth(100));
            GUILayout.Label("Using Editor Mask", labelStyle);
            GUI.enabled = EnableMask;
            EditorName = GUILayout.TextField(EditorName, GUILayout.MinWidth(50));
            GUI.enabled = true;
            EnableMask = EditorGUILayout.ToggleLeft(string.Empty, EnableMask, GUILayout.Width(20));
            var orgColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(253 / 255f, 163 / 255f, 255 / 255f);
            if (GUILayout.Button("Create Node Type Registry"))
            {
                EditorApplication.delayCall += WriteNodeTypeRegistry;
            }
            GUI.backgroundColor = orgColor;
            GUILayout.EndHorizontal();
        }
        private void DrawCompileResult(int state)
        {
            if (state == 1)
            {
                GUILayout.Label("<color=#3aff48>Compiler</color> : Compile Succeed!", labelStyle);
            }
            if (state == 0)
            {
                GUILayout.Label("<color=#ff2f2f>Compiler</color> : Compile Failed!", labelStyle);
            }
            if (state == -1)
            {
                GUILayout.Label("<color=#ff2f2f>Compiler</color> : Input Code Is Empty!", labelStyle);
            }
            if (state == -2)
            {
                GUILayout.Label("<color=#ff2f2f>Compiler</color> : Type Dictionary has not generated!", labelStyle);
            }
        }
        private void DrawDecompileResult(int state)
        {
            if (state == 1)
            {
                GUILayout.Label("<color=#3aff48>AkiBTDecompiler</color> : Decompile Succeed!", labelStyle);
            }
            if (state == 0)
            {
                GUILayout.Label("<color=#ff2f2f>AkiBTDecompiler</color> : Decompile Failed!", labelStyle);
            }
            if (state == -1)
            {
                GUILayout.Label("<color=#ff2f2f>AkiBTDecompiler</color> : Input Object's type is not supported!", labelStyle);
            }
            if (state == -2)
            {
                GUILayout.Label("<color=#ff2f2f>AkiBTDecompiler</color> : Type Dictionary has not generated!", labelStyle);
            }
        }
        private int Compile()
        {
            string fileInStreaming = $"{Application.streamingAssetsPath}/{RegistryNameName}.json";
            if (!File.Exists(fileInStreaming))
            {
                return -2;
            }
            if (string.IsNullOrEmpty(InputCode))
            {
                return -1;
            }
            var bt = new Compiler(fileInStreaming).Verbose(true).Compile(InputCode);
            _outputTree = CreateInstance<BehaviorTreeAsset>();
            _outputTree.SetBehaviorTreeData(bt.GetData());
            return 1;
        }
        private int Decompile(IBehaviorTreeContainer behaviorTreeContainer)
        {
            string fileInStreaming = $"{Application.streamingAssetsPath}/{RegistryNameName}.json";
            if (!File.Exists(fileInStreaming))
            {
                return -2;
            }
            if (behaviorTreeContainer == null) return -1;
            InputCode = new Decompiler().Decompile(behaviorTreeContainer);
            return 1;
        }
        private void WriteNodeTypeRegistry()
        {
            IEnumerable<Type> list = SubclassSearchUtility.FindSubClassTypes(typeof(NodeBehavior));
            string[] showGroups = null;
            string[] notShowGroups = null;
            if (EnableMask) (showGroups, notShowGroups) = BehaviorTreeSetting.GetOrCreateSettings().GetMask(EditorName);
            if (showGroups != null)
            {
                for (int i = 0; i < showGroups.Length; i++) Log($"Showing Group:{showGroups[i]}");
            }
            if (notShowGroups != null)
            {
                for (int i = 0; i < notShowGroups.Length; i++) Log($"Not Showing Group:{notShowGroups[i]}");
            }
            var groups = list.GroupsByAkiGroup();
            list = list.Except(groups.SelectMany(x => x)).ToList();
            groups = groups.SelectGroup(showGroups).ExceptGroup(notShowGroups);
            list = list.Concat(groups.SelectMany(x => x).Distinct()).Concat(SubclassSearchUtility.FindSubClassTypes(typeof(SharedVariable))); ;
            var registry = new NodeTypeRegistry();
            foreach (var type in list)
            {
                AddTypeInfo(registry, type);
            }
            string path = $"{Application.streamingAssetsPath}/{RegistryNameName}.json";
            if (!File.Exists(Application.streamingAssetsPath))
            {
                Directory.CreateDirectory(Application.streamingAssetsPath);
            }
            //Write to file
            File.WriteAllText(path, JsonConvert.SerializeObject(registry, Formatting.Indented), System.Text.Encoding.UTF8);
            Log($"Create node type registry succeed, saving path: {path}");
        }
        private static void AddTypeInfo(NodeTypeRegistry dict, Type type)
        {
            string path = type.Name;
            // Generate a node path as simple as possible
            if (dict.TryGetNode(path, out _))
            {
                string newPath = $"{type.Namespace}.{path}";
                Log($"{path} already exits, append namespace to path: {newPath}");
                path = newPath;
                if (dict.TryGetNode(path, out _))
                {
                    newPath = $"{type.Assembly.GetName().Name}.{path}";
                    Log($"{path} already exits, append assembly name to path: {newPath}");
                    path = newPath;
                }
            }
            dict.SetNode(path, GenerateTypeInfo(type));
        }
        private static NodeInfo GenerateTypeInfo(Type type)
        {
            var enumDict = new Dictionary<Type, int>();
            var info = new NodeInfo
            {
                className = type.Name,
                ns = type.Namespace,
                asm = type.Assembly.GetName().Name
            };
            if (type.IsSubclassOf(typeof(SharedVariable)))
            {
                info.isVariable = true;
                return info;
            }
            info.isVariable = false;
            var properties = NodeTypeRegistry.GetAllFields(type).ToList();
            if (properties.Count == 0) return info;
            info.properties = new();
            properties.ForEach((p) =>
                {
                    var label = p.GetCustomAttribute(typeof(AkiLabelAttribute), false) as AkiLabelAttribute;
                    PropertyInfo propertyInfo;
                    info.properties.Add(propertyInfo = new PropertyInfo()
                    {
                        name = p.Name,
                        label = label?.Title ?? p.Name,
                        // Get a fast field type
                        fieldType = NodeTypeRegistry.GetFieldType(p.FieldType)
                    });
                });
            return info;
        }
        private static void Log(string message)
        {
            Debug.Log($"<color=#3aff48>Compiler</color> :{message}");
        }
        private static void LogError(string message)
        {
            Debug.Log($"<color=#ff2f2f>Compiler</color> :{message}");
        }
    }
}