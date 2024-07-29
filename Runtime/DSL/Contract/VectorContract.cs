using System;
using UnityEngine;
namespace Kurisu.AkiBT.DSL
{
    public class Vector3IntToVector3Contract : ITypeContract
    {
        public bool CanConvert(Type inputType, Type expectType)
        {
            return (inputType == typeof(Vector3Int)) && expectType == typeof(Vector3);
        }

        public object Convert(in object value, Type inputType, Type expectType)
        {
            return (Vector3)(Vector3Int)value;
        }
    }
    public class Vector2ToVector3Contract : ITypeContract
    {
        public bool CanConvert(Type inputType, Type expectType)
        {
            return (inputType == typeof(Vector2)) && expectType == typeof(Vector3);
        }

        public object Convert(in object value, Type inputType, Type expectType)
        {
            return (Vector3)(Vector2)value;
        }
    }

    public class Vector3ToVector2Contract : ITypeContract
    {
        public bool CanConvert(Type inputType, Type expectType)
        {
            return (inputType == typeof(Vector3)) && expectType == typeof(Vector2);
        }

        public object Convert(in object value, Type inputType, Type expectType)
        {
            return (Vector2)(Vector3)value;
        }
    }
    public class Vector2IntToVector2Contract : ITypeContract
    {
        public bool CanConvert(Type inputType, Type expectType)
        {
            return (inputType == typeof(Vector2Int)) && expectType == typeof(Vector2);
        }

        public object Convert(in object value, Type inputType, Type expectType)
        {
            return (Vector2)(Vector2Int)value;
        }
    }
}