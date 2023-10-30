using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
namespace Kurisu.AkiBT.Editor
{
    public class ServiceEditorWindow : EditorWindow
    {
        [MenuItem("Tools/AkiBT/AkiBT Service")]
        private static void ShowEditorWindow()
        {
            GetWindow<ServiceEditorWindow>("AkiBT Service");
        }
        private NodeTypeSearchWindow searchWindow;
        private BehaviorTreeSearchCache searchCache;
        private Vector2 m_ScrollPosition;
        public delegate Vector2 BeginVerticalScrollViewFunc(Vector2 scrollPosition, bool alwaysShowVertical, GUIStyle verticalScrollbar, GUIStyle background, params GUILayoutOption[] options);
        static BeginVerticalScrollViewFunc s_func;
        private int mTab;
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
        private SerializedObject searchCacheSerializedObject;
        private BehaviorTreeServiceData serviceData;
        private SerializedObject serviceSerializedObject;
        private Type searchType;
        private SerializedProperty collectionProperty;
        private void OnEnable()
        {
            searchCache = CreateInstance<BehaviorTreeSearchCache>();
            searchCache.allBehaviorTreeSOCache = BehaviorTreeSearchUtility.GetAllBehaviorTreeSO();
            searchCacheSerializedObject = new SerializedObject(searchCache);
            searchWindow = CreateInstance<NodeTypeSearchWindow>();
            searchWindow.Initialize((T) => searchType = T);
            serviceData = BehaviorTreeSetting.GetOrCreateSettings().ServiceData;
            serviceData.ForceSetUp();
            serviceSerializedObject = new SerializedObject(serviceData);
            collectionProperty = serviceSerializedObject.FindProperty("serializationCollection.serializationPairs");
        }
        private void OnGUI()
        {
            serviceSerializedObject.Update();
            m_ScrollPosition = BeginVerticalScrollView(m_ScrollPosition, false, GUI.skin.verticalScrollbar, "OL Box");
            mTab = GUILayout.Toolbar(mTab, new string[] { "Serialization Service", "Searching Service" });
            switch (mTab)
            {
                case 0:
                    DrawSerializationService();
                    break;
                case 1:
                    DrawSearchingService();
                    break;
            }
        }
        private void DrawSerializationService()
        {
            EditorGUILayout.PropertyField(collectionProperty);
            EditorGUILayout.EndScrollView();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Serialize All Files"))
            {
                ExportAll();
            }
            if (GUILayout.Button("Import Json Files"))
            {
                ImportAll();
            }
            if (GUILayout.Button("Deserialize All"))
            {
                if (EditorUtility.DisplayDialog("User Tips", "Deserialize all files will change behaviorTreeSO on the left, make sure to have a backup", "Still Deserialize", "Cancel"))
                {
                    foreach (var pair in serviceData.serializationCollection.serializationPairs)
                    {
                        if (pair.serializedData != null && pair.behaviorTreeSO != null)
                        {
                            pair.behaviorTreeSO.Deserialize(pair.serializedData.text);
                            EditorUtility.SetDirty(pair.behaviorTreeSO);
                        }
                    }
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
            if (GUILayout.Button(new GUIContent("Unbind All", "This will unbind all json files, not delate")))
            {
                foreach (var pair in serviceData.serializationCollection.serializationPairs)
                {
                    pair.serializedData = null;
                }
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            EditorGUILayout.EndHorizontal();
        }
        private void DrawSearchingService()
        {
            searchCacheSerializedObject.Update();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label($"Search Name");
            string newName = GUILayout.TextField(searchCache.searchName);
            EditorGUILayout.EndHorizontal();
            string typeName = searchType?.FullName ?? "<Null>";
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label($"Select Type : {typeName}");
            if (GUILayout.Button("Select Type"))
            {
                SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), searchWindow);
            }
            EditorGUILayout.EndHorizontal();
            if (searchType != searchCache.searchType || newName != searchCache.searchName)
            {
                searchCache.searchName = newName;
                searchCache.searchType = searchType;
                if (searchType == null && string.IsNullOrEmpty(newName))
                {
                    searchCache.searchResult = new();
                }
                else
                {
                    //Use cache for quick search
                    searchCache.searchResult = BehaviorTreeSearchUtility.SearchBehaviorTreeSO(searchType, serviceData, searchCache.allBehaviorTreeSOCache);
                    if (!string.IsNullOrEmpty(newName))
                        searchCache.searchResult = searchCache.searchResult.Where(x => x.behaviorTreeSO.name.Contains(newName)).ToList();
                }
                searchCacheSerializedObject.ApplyModifiedProperties();
            }
            EditorGUILayout.PropertyField(searchCacheSerializedObject.FindProperty("searchResult"));
            EditorGUILayout.EndScrollView();
        }
        private void ExportAll()
        {
            string path = EditorUtility.OpenFolderPanel("Choose json files saving path", Application.dataPath, "");
            if (string.IsNullOrEmpty(path)) return;
            List<string> savePaths = new();
            for (int i = 0; i < serviceData.serializationCollection.serializationPairs.Count; i++)
            {
                var pair = serviceData.serializationCollection.serializationPairs[i];
                var serializedData = SerializeUtility.SerializeTree(pair.behaviorTreeSO, true, true);
                string folderPath = path + $"/{pair.behaviorTreeSO.GetType().Name}";
                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
                string savePath = $"{folderPath}/{pair.behaviorTreeSO.name}_{serviceData.serializationCollection.guids[i]}.json";
                savePaths.Add(savePath);
                File.WriteAllText(savePath, serializedData);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            for (int i = 0; i < serviceData.serializationCollection.serializationPairs.Count; i++)
            {
                serviceData.serializationCollection.serializationPairs[i].serializedData = AssetDatabase.LoadAssetAtPath<TextAsset>(GetRelativePath(savePaths[i]));
            }
            serviceSerializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(serviceData);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        private void ImportAll()
        {
            string path = EditorUtility.OpenFolderPanel("Select json files import path", Application.dataPath, "");
            if (string.IsNullOrEmpty(path)) return;
            string[] subDirectories = Directory.GetDirectories(path);
            List<string> configPaths = new();
            foreach (var directory in subDirectories)
            {
                string[] files = Directory.GetFiles(directory);
                foreach (var file in files)
                {
                    if (Path.GetExtension(file) == ".json")
                    {
                        configPaths.Add(file);
                    }
                }
            }
            var jsonFiles = configPaths.Select(x => AssetDatabase.LoadAssetAtPath<TextAsset>(GetRelativePath(x))).ToHashSet();
            serviceData.serializationCollection.InjectJsonFiles(jsonFiles);
            serviceSerializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(serviceData);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        private static string GetRelativePath(string path)
        {
            return "Assets/" + path.Replace(Application.dataPath, string.Empty);
        }
    }
}
