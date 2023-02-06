using System.Reflection;
using UnityEditor.UIElements;
using System;
using UnityEngine.UIElements;
namespace Kurisu.AkiBT.Editor
{
public class SharedFloatResolver :FieldResolver<SharedFloatField,SharedFloat>
    {
        public SharedFloatResolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override void SetTree(BehaviorTreeView ownerTreeView)
        {
            editorField.InitField(ownerTreeView);
        }
        private SharedFloatField editorField;
        protected override SharedFloatField CreateEditorField(FieldInfo fieldInfo)
        {
            editorField = new SharedFloatField(fieldInfo.Name,null,fieldInfo.FieldType);
            return editorField;
        }
        public static bool IsAcceptable(Type infoType,FieldInfo info)=>infoType==typeof(SharedFloat) ;
         
    }
  public class SharedFloatField : SharedVariableField<SharedFloat,float>
    {
       
        public SharedFloatField(string label, VisualElement visualInput, Type objectType) : base(label, visualInput,objectType)
        {

        }
        protected override BaseField<float> CreateValueField()=>new FloatField();
    }
}