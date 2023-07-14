using System.IO;
using UnityEditor;
using UnityEngine;
namespace Kurisu.AkiBT.Editor
{
    public class BehaviorTreeUserServiceData : ScriptableObject
    {
        public BehaviorTreeSerializationCollection serializationCollection=new();
        public void ForceSetUp()
        {
            serializationCollection.SetUp();
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
