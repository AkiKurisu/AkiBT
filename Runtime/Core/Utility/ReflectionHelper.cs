using System;
using System.Collections.Generic;
using System.Reflection;
namespace Kurisu.AkiBT
{
    /// <summary>
    /// Code from https://stackoverflow.com/questions/39092168/c-sharp-copying-unityevent-information-using-reflection
    /// </summary>
    public static class ReflectionHelper
    {
        /// <summary>
        /// Gets all fields from an object and its hierarchy inheritance.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="flags">The flags.</param>
        /// <returns>All fields of the type.</returns>
        public static List<FieldInfo> GetAllFields(this Type type, BindingFlags flags)
        {
            // Early exit if Object type
            if (type == typeof(object))
            {
                return new List<FieldInfo>();
            }

            // Recursive call
            var fields = type.BaseType.GetAllFields(flags);
            fields.AddRange(type.GetFields(flags | BindingFlags.DeclaredOnly));
            return fields;
        }

        /// <summary>
        /// Perform a deep copy of the class.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <returns>A deep copy of obj.</returns>
        /// <exception cref="ArgumentNullException">Object cannot be null</exception>
        public static T DeepCopy<T>(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("Object cannot be null");
            }
            return (T)DoCopy(obj);
        }


        /// <summary>
        /// Does the copy.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Unknown type</exception>
        private static object DoCopy(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            // Value type
            var type = obj.GetType();
            if (type.IsValueType || type == typeof(string))
            {
                return obj;
            }

            // Array
            else if (type.IsArray)
            {
                Type elementType = type.GetElementType();
                var array = obj as Array;
                Array copied = Array.CreateInstance(elementType, array.Length);
                for (int i = 0; i < array.Length; i++)
                {
                    copied.SetValue(DoCopy(array.GetValue(i)), i);
                }
                return Convert.ChangeType(copied, obj.GetType());
            }

            // Unity Object
            else if (typeof(UnityEngine.Object).IsAssignableFrom(type))
            {
                return obj;
            }

            // Class -> Recursion
            else if (type.IsClass)
            {
                var copy = Activator.CreateInstance(obj.GetType());

                var fields = type.GetAllFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (FieldInfo field in fields)
                {
                    var fieldValue = field.GetValue(obj);
                    if (fieldValue != null)
                    {
                        field.SetValue(copy, DoCopy(fieldValue));
                    }
                }

                return copy;
            }

            // Fallback
            else
            {
                throw new ArgumentException("Unknown type");
            }
        }
    }
}
