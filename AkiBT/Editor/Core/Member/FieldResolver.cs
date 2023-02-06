using System;
using System.Reflection;
using UnityEngine.UIElements;
using UnityEngine;
using System.Collections.Generic;
namespace Kurisu.AkiBT.Editor
{
    sealed class Ordered : Attribute
    {
        public int Order = 100;
    }

    public interface IFieldResolver
    {
        /// <summary>
        /// 获取ValueField同时绑定行为树视图
        /// </summary>
        /// <param name="ownerTreeView"></param>
        /// <returns></returns>
        VisualElement GetEditorField(BehaviorTreeView ownerTreeView);
        /// <summary>
        /// 获取ValueField同时绑定共享变量
        /// </summary>
        /// <param name="ExposedProperties"></param>
        /// <param name="variable"></param>
        /// <returns></returns>
        VisualElement GetEditorField(List<SharedVariable> ExposedProperties,SharedVariable variable);
        /// <summary>
        /// 只创建ValueField,不进行任何绑定
        /// </summary>
        /// <returns></returns>
        public VisualElement CreateField();
        void Restore(NodeBehavior behavior);
        void Commit(NodeBehavior behavior);   
        void Copy(IFieldResolver resolver);
        object Value{get;} 
    }

    public abstract class FieldResolver<T, K> :IFieldResolver where T: BaseField<K>
    {
        private readonly FieldInfo fieldInfo;
        private T editorField;
        public object Value=>editorField.value;
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
            TooltipAttribute tooltip=this.fieldInfo.GetCustomAttribute<TooltipAttribute>();
            if(tooltip!=null)this.editorField.tooltip=tooltip.tooltip;
        }

        protected abstract T CreateEditorField(FieldInfo fieldInfo);
        public VisualElement CreateField()=>CreateEditorField(this.fieldInfo);
        protected virtual void SetTree(BehaviorTreeView ownerTreeView){}
        public VisualElement GetEditorField(BehaviorTreeView ownerTreeView)
        {
            SetTree(ownerTreeView);
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
        public void Copy(IFieldResolver resolver)
        {
            if(resolver is not FieldResolver<T, K>)return;
            editorField.value=(K)resolver.Value;
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