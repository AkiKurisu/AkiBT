using System.Reflection;
using System;
using UnityEngine.UIElements;
namespace Kurisu.AkiBT.Editor
{

    public class SharedBoolResolver :FieldResolver<SharedBoolField,SharedBool>
    {
        public SharedBoolResolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override void SetTree(BehaviorTreeView ownerTreeView)
        {
            editorField.InitField(ownerTreeView);
        }
        private SharedBoolField editorField;
        protected override SharedBoolField CreateEditorField(FieldInfo fieldInfo)
        {
            editorField = new SharedBoolField(fieldInfo.Name,null,fieldInfo.FieldType);
            return editorField;
        }
        public static bool IsAcceptable(Type infoType,FieldInfo info)=>infoType==typeof(SharedBool) ;
         
    }
     public class SharedBoolField : SharedVariableField<SharedBool,bool>
    {
        public SharedBoolField(string label, VisualElement visualInput, Type objectType) : base(label, visualInput,objectType)
        {
           

        }

        protected override BaseField<bool> CreateValueField()=>new Toggle("Value");
    }
}
