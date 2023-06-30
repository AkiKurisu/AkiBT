#if UNITY_EDITOR
using Newtonsoft.Json.Linq;
using System.Linq;
using Newtonsoft.Json;
#endif
using UnityEngine;
namespace Kurisu.AkiBT
{
    public class BehaviorTreeSerializeUtility
    {
        public static string SerializeTree(IBehaviorTree behaviorTree,bool indented=false,bool serializeEditorData=false)
        {
            return TemplateToIL(TreeToTemplate(behaviorTree),indented,serializeEditorData);
        }
        public static BehaviorTreeTemplate DeserializeTree(string serializedData)
        {
            return JsonUtility.FromJson<BehaviorTreeTemplate>(serializedData);
        }
        public static BehaviorTreeTemplate TreeToTemplate(IBehaviorTree behaviorTree)
        {
            var template=new BehaviorTreeTemplate(behaviorTree);
            return template;
        }
        public static string TemplateToIL(BehaviorTreeTemplate template,bool indented=false,bool serializeEditorData=false)
        {
            var json=JsonUtility.ToJson(template);
            #if UNITY_EDITOR
                //Remove editor only fields in behaviorTree manually
                JObject obj = JObject.Parse(json);
                if(!serializeEditorData)
                foreach (JProperty prop in obj.Descendants().OfType<JProperty>().ToList())
                {
                    if (prop.Name == "graphPosition"||prop.Name == "description"||prop.Name == "guid")
                    {
                        prop.Remove();
                    }
                }
                return obj.ToString(indented?Formatting.Indented:Formatting.None);
            #else
                //Don't need remove in build version as those fields won't be serialized
                return json;
            #endif
        }
    }
}
