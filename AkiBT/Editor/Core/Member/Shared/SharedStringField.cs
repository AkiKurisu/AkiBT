using System.Reflection;
using System;
using UnityEngine.UIElements;
namespace Kurisu.AkiBT.Editor
{
public class SharedStringResolver :FieldResolver<SharedStringField,SharedString>
    {
        public SharedStringResolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override void SetTree(BehaviorTreeView ownerTreeView)
        {
            editorField.InitField(ownerTreeView);
        }
        private SharedStringField editorField;
        protected override SharedStringField CreateEditorField(FieldInfo fieldInfo)
        {
            editorField = new SharedStringField(fieldInfo.Name,null,fieldInfo.FieldType);
            return editorField;
        }
        public static bool IsAcceptable(Type infoType,FieldInfo info)=>infoType ==typeof(SharedString) ;
         
    }
    public class SharedStringField : SharedVariableField<SharedString,string>
    {
         
        public SharedStringField(string label, VisualElement visualInput, Type objectType) : base(label, visualInput,objectType)
        {
        }
        protected override BaseField<string> CreateValueField()=>new TextField();
    }
}