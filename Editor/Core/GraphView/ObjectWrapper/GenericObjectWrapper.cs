using System;
using UnityEngine;
namespace Kurisu.AkiBT.Editor
{
    // Code from Unity.Kinematica
    internal interface IValue
    {
    }

    internal interface IValueStore<T> : IValue
    {
        T Value { get; set; }
    }

    [Serializable]
    public class GenericObjectWrapper<T> : ScriptableObject, IValueStore<T>
    {
        [SerializeField]
        T m_Value;

        public T Value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }
    }
}
