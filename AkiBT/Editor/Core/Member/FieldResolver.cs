using System;
using System.Reflection;
using UnityEngine.UIElements;
using System.Collections.Generic;
namespace Kurisu.AkiBT.Editor
{
    sealed class Ordered : Attribute
    {
        public int Order = 100;
    }

    public interface IFieldResolver
    {
        VisualElement GetEditorField( );
        VisualElement GetEditorField(List<SharedVariable> ExposedProperties,SharedVariable variable);

        void Restore(NodeBehavior behavior);

        void Commit(NodeBehavior behavior);
       
        
    }

    public abstract class FieldResolver<T, K> :IFieldResolver where T: BaseField<K>
    {
        private readonly FieldInfo fieldInfo;
        private T editorField;

        protected FieldResolver(FieldInfo fieldInfo)
        {
            this.fieldInfo = fieldInfo;
            SetEditorField();
        }

        private void SetEditorField()
        {
            this.editorField = this.CreateEditorField(this.fieldInfo);
            //修改标签
            AkiLabel label=this.fieldInfo.GetCustomAttribute<AkiLabel>();
            if(label!=null)this.editorField.label=label.Title;
        }

        protected abstract T CreateEditorField(FieldInfo fieldInfo);
        public VisualElement GetEditorField()
        {

            return this.editorField;
        }
        /// <summary>
        /// 不安全的方法,外界注入回调操作的列表和共享变量,不保证转换成功
        /// </summary>
        /// <param name="ExposedProperties"></param>
        /// <param name="variable"></param>
        /// <returns></returns>
        public VisualElement GetEditorField(List<SharedVariable> ExposedProperties,SharedVariable variable)
        {
            this.editorField.RegisterValueChangedCallback(evt =>
            {
                var index = ExposedProperties.FindIndex(x => x.Name == variable.Name);
                ExposedProperties[index].SetValue(evt.newValue) ;
            });
            this.editorField.value=(K)variable.GetValue();
            return this.editorField;
        }
       
        public void Restore(NodeBehavior behavior)
        {
            editorField.value = (K)fieldInfo.GetValue(behavior);
        }

        public void Commit(NodeBehavior behavior)
        {
           fieldInfo.SetValue(behavior, editorField.value);
        }
    }
}