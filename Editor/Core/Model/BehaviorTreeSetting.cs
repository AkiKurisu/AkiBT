using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
namespace Kurisu.AkiBT.Editor
{
    [Serializable]
    internal class EditorSetting
    {
        [Tooltip("Editor implementation that this setting belongs to, according to field `TreeEditorName` in IBehaviorTreeView")]
        public string EditorName = "AkiBT";
        [AkiGroupSelector, Tooltip("Display type, filter AkiGroup according to this list, nodes without category will always be displayed")]
        public string[] ShowGroups = new string[0];
        [AkiGroupSelector, Tooltip("The type that is not displayed, filter the AkiGroup according to this list, and the nodes without categories will always be displayed")]
        public string[] NotShowGroups = new string[0];
        [Tooltip("You can customize the style of the Graph view")]
        public StyleSheet graphStyleSheet;
        [Tooltip("You can customize the style of the Inspector inspector")]
        public StyleSheet inspectorStyleSheet;
        [Tooltip("You can customize the style of Node nodes")]
        public StyleSheet nodeStyleSheet;
    }
    public class BehaviorTreeSetting : ScriptableObject
    {
        public const string Version = "v1.4.8";
        private const string k_BehaviorTreeSettingsPath = "Assets/AkiBTSetting.asset";
        private const string k_UserServiceSettingPath = "Assets/AkiBTUserServiceData.asset";
        private const string GraphFallBackPath = "AkiBT/Graph";
        private const string InspectorFallBackPath = "AkiBT/Inspector";
        private const string NodeFallBackPath = "AkiBT/Node";

        [SerializeField, Tooltip("Editor configuration, you can use different styles based on the editor name and provide filtering solutions for node search")]
        private EditorSetting[] settings;
        [SerializeField]
        private float autoLayoutSiblingDistance = 50f;
        [SerializeField]
        private bool jsonSerializeEditorData = true;
        [SerializeField, HideInInspector]
        private bool autoSave;
        [SerializeField, HideInInspector]
        private string lastPath;
        [SerializeField, HideInInspector]
        private BehaviorTreeServiceData serviceData;
        public BehaviorTreeServiceData ServiceData
        {
            get
            {
                if (serviceData == null)
                {
                    var guids = AssetDatabase.FindAssets($"t:{nameof(BehaviorTreeServiceData)}");
                    if (guids.Length == 0)
                    {
                        serviceData = CreateInstance<BehaviorTreeServiceData>();
                        Debug.Log($"AkiBT User Service Data saving path : {k_UserServiceSettingPath}");
                        AssetDatabase.CreateAsset(serviceData, k_UserServiceSettingPath);
                        AssetDatabase.SaveAssets();
                    }
                    else serviceData = AssetDatabase.LoadAssetAtPath<BehaviorTreeServiceData>(AssetDatabase.GUIDToAssetPath(guids[0]));
                }
                return serviceData;
            }
        }
        /// <summary>
        /// Cache last open folder path in editor
        /// </summary>
        /// <value></value>
        public string LastPath
        {
            get => lastPath;
            set => lastPath = value;

        }
        /// <summary>
        /// Cache user auto save option value
        /// </summary>
        /// <value></value>
        public bool AutoSave
        {
            get => autoSave;
            set => autoSave = value;
        }
        /// <summary>
        /// Auto node layout sibling distance
        /// </summary> <summary>
        public float AutoLayoutSiblingDistance => autoLayoutSiblingDistance;
        public bool JsonSerializeEditorData => jsonSerializeEditorData;
        public StyleSheet GetGraphStyle(string editorName)
        {
            if (settings == null || settings.Length == 0 || !settings.Any(x => x.EditorName.Equals(editorName))) return Resources.Load<StyleSheet>(GraphFallBackPath);
            var editorSetting = settings.First(x => x.EditorName.Equals(editorName));
            return editorSetting.graphStyleSheet != null ? editorSetting.graphStyleSheet : Resources.Load<StyleSheet>(GraphFallBackPath);
        }
        public StyleSheet GetInspectorStyle(string editorName)
        {
            if (settings == null || settings.Length == 0 || !settings.Any(x => x.EditorName.Equals(editorName))) return Resources.Load<StyleSheet>(InspectorFallBackPath);
            var editorSetting = settings.First(x => x.EditorName.Equals(editorName));
            return editorSetting.inspectorStyleSheet != null ? editorSetting.inspectorStyleSheet : Resources.Load<StyleSheet>(InspectorFallBackPath);
        }
        public StyleSheet GetNodeStyle(string editorName)
        {
            if (settings == null || settings.Length == 0 || !settings.Any(x => x.EditorName.Equals(editorName))) return Resources.Load<StyleSheet>(NodeFallBackPath);
            var editorSetting = settings.First(x => x.EditorName.Equals(editorName));
            return editorSetting.nodeStyleSheet != null ? editorSetting.nodeStyleSheet : Resources.Load<StyleSheet>(NodeFallBackPath);
        }
        private static readonly string[] internalNotShowGroups = new string[1] { "Hidden" };
        public (string[] showGroups, string[] notShowGroups) GetMask(string editorName)
        {
            if (settings == null || settings.Length == 0 || !settings.Any(x => x.EditorName.Equals(editorName))) return (null, internalNotShowGroups);
            var editorSetting = settings.First(x => x.EditorName.Equals(editorName));
            return (editorSetting.ShowGroups, editorSetting.NotShowGroups.Concat(internalNotShowGroups).ToArray());
        }
        public static BehaviorTreeSetting GetOrCreateSettings()
        {
            var guids = AssetDatabase.FindAssets($"t:{nameof(BehaviorTreeSetting)}");
            BehaviorTreeSetting setting = null;
            if (guids.Length == 0)
            {
                setting = CreateInstance<BehaviorTreeSetting>();
                Debug.Log($"AkiBT Setting saving path : {k_BehaviorTreeSettingsPath}");
                AssetDatabase.CreateAsset(setting, k_BehaviorTreeSettingsPath);
                AssetDatabase.SaveAssets();
            }
            else setting = AssetDatabase.LoadAssetAtPath<BehaviorTreeSetting>(AssetDatabase.GUIDToAssetPath(guids[0]));
            return setting;
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }
    }

    internal class BehaviorTreeSettingsProvider : SettingsProvider
    {
        private SerializedObject m_Settings;
        private class Styles
        {
            public static GUIContent GraphEditorSettingStyle = new("Graph Editor Setting");
            public static GUIContent LayoutDistanceStyle = new("Layout Distance", "Auto node layout sibling distance");
            public static GUIContent SerializeEditorDataStyle = new("Serialize Editor Data", "Serialize node editor data when use json serialization, turn off to decrease file size");
        }
        public BehaviorTreeSettingsProvider(string path, SettingsScope scope = SettingsScope.User) : base(path, scope) { }
        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            m_Settings = BehaviorTreeSetting.GetSerializedSettings();
        }
        public override void OnGUI(string searchContext)
        {
            GUILayout.BeginVertical("Editor Settings", GUI.skin.box);
            GUILayout.Space(EditorGUIUtility.singleLineHeight);
            EditorGUILayout.PropertyField(m_Settings.FindProperty("settings"), Styles.GraphEditorSettingStyle);
            EditorGUILayout.PropertyField(m_Settings.FindProperty("autoLayoutSiblingDistance"), Styles.LayoutDistanceStyle);
            EditorGUILayout.PropertyField(m_Settings.FindProperty("jsonSerializeEditorData"), Styles.SerializeEditorDataStyle);
            m_Settings.ApplyModifiedPropertiesWithoutUndo();
            GUILayout.EndVertical();
        }
        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {

            var provider = new BehaviorTreeSettingsProvider("Project/AkiBT Settings", SettingsScope.Project)
            {
                keywords = GetSearchKeywordsFromGUIContentProperties<Styles>()
            };
            return provider;

        }
    }
}