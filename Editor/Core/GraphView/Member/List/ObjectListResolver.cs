using System.Reflection;
namespace Kurisu.AkiBT.Editor
{
    public class ObjectListResolver<T> : ListResolver<T> where T : UnityEngine.Object
    {
        public ObjectListResolver(FieldInfo fieldInfo) : base(fieldInfo, new ObjectResolver(fieldInfo))
        {

        }
        protected override ListField<T> CreateEditorField(FieldInfo fieldInfo)
        {
            return new ObjectListField<T>(fieldInfo.Name, () => childResolver.CreateField(), () => null);
        }
    }
}