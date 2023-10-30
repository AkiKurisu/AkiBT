#if UNITY_EDITOR
using UnityEditor;
using Newtonsoft.Json.Linq;
using System.Linq;
using Newtonsoft.Json;
#endif
using UnityEngine;
namespace Kurisu.AkiBT
{
    public class SerializeUtility
    {
        public static string SerializeTree(IBehaviorTree behaviorTree, bool indented = false, bool serializeEditorData = false)
        {
            return TemplateToIL(TreeToTemplate(behaviorTree), indented, serializeEditorData);
        }
        public static BehaviorTreeTemplate DeserializeTree(string serializedData)
        {
#if UNITY_EDITOR
            if (!string.IsNullOrEmpty(serializedData))
            {
                JObject obj = JObject.Parse(serializedData);
                foreach (JProperty prop in obj.Descendants().OfType<JProperty>().ToList())
                {
                    if (prop.Name == "instanceID")
                    {
                        var UObject = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath((string)prop.Value));
                        if (UObject == null)
                        {
                            prop.Value = 0;
                            continue;
                        }
                        prop.Value = UObject.GetInstanceID();
                    }
                }
                return JsonUtility.FromJson<BehaviorTreeTemplate>(obj.ToString(Formatting.Indented));
            }
#endif
            return JsonUtility.FromJson<BehaviorTreeTemplate>(serializedData);
        }
        public static BehaviorTreeTemplate TreeToTemplate(IBehaviorTree behaviorTree)
        {
            var template = new BehaviorTreeTemplate(behaviorTree);
            return template;
        }
        public static string TemplateToIL(BehaviorTreeTemplate template, bool indented = false, bool serializeEditorData = false)
        {
            var json = JsonUtility.ToJson(template);
#if UNITY_EDITOR
            JObject obj = JObject.Parse(json);
            foreach (JProperty prop in obj.Descendants().OfType<JProperty>().ToList())
            {
                //Remove editor only fields in behaviorTree manually
                if (!serializeEditorData)
                    if (prop.Name == "graphPosition" || prop.Name == "description" || prop.Name == "guid")
                    {
                        prop.Remove();
                    }
                if (prop.Name == "instanceID")
                {
                    string propertyName = prop.Name;
                    if (prop.Parent?.Parent != null) propertyName = (prop.Parent?.Parent as JProperty).Name;
                    var UObject = EditorUtility.InstanceIDToObject((int)prop.Value);
                    if (UObject == null) continue;
                    string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(UObject));
                    if (string.IsNullOrEmpty(guid))
                    {
                        Debug.LogWarning($"<color=#fcbe03>{template.TemplateName}</color> :  Can't serialize UnityEngine.Object field : {propertyName}");
                        continue;
                    }
                    //Convert to GUID
                    prop.Value = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(UObject));
                }
            }
            return obj.ToString(indented ? Formatting.Indented : Formatting.None);
#else
            return json;
#endif
        }
    }
}
