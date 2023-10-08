using System;
using UnityEngine;
namespace Kurisu.AkiBT.Editor
{
    // Code from Unity.Kinematica
    internal static class GenericObjectWrapperHelper
    {
        public static ScriptableObject Wrap<T>()
        {
            Type genericType = typeof(GenericObjectWrapper<>).MakeGenericType(typeof(T));
            Type dynamicType = DynamicTypeBuilder.MakeDerivedType<T>(genericType);

            var dynamicTypeInstance = ScriptableObject.CreateInstance(dynamicType);
            if (dynamicTypeInstance is not IValueStore<T> valueStore)
            {
                return null;
            }
            valueStore.Value = default;

            return dynamicTypeInstance;
        }
    }
}
