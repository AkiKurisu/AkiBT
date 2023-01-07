using System.Reflection;
using UnityEditor.UIElements;
using System;
using UnityEngine.UIElements;
namespace Kurisu.AkiBT.Editor
{
public class SharedIntResolver :FieldResolver<SharedIntField,SharedInt>
    {
        public SharedIntResolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override void SetTree(BehaviorTreeView ownerTreeView)
        {
            editorField.InitField(ownerTreeView);
        }
        private SharedIntField editorField;
        protected override SharedIntField CreateEditorField(FieldInfo fieldInfo)
        {
            editorField = new SharedIntField(fieldInfo.Name,null,fieldInfo.FieldType);
            return editorField;
        }
        public static bool IsAcceptable(Type infoType,FieldInfo info)=>infoType ==typeof(SharedInt) ;
         
    }
    public class SharedIntField : SharedVariableField<SharedInt,int>
    {
         
        public SharedIntField(string label, VisualElement visualInput, Type objectType) : base(label, visualInput,objectType)
        {
        }
        protected override BaseField<int> CreateValueField()=>new IntegerField();
    }
}