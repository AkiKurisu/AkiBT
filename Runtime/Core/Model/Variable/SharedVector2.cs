using System;
using UnityEngine;
namespace Kurisu.AkiBT
{
    [Serializable]
    public class SharedVector2 : SharedVariable<Vector2>
    {
        public SharedVector2(Vector2 value)
        {
            this.value = value;
        }
        public SharedVector2()
        {

        }
        protected override SharedVariable<Vector2> CloneT()
        {
            return new SharedVector2() { Value = value };
        }
    }
    [Serializable]
    public class SharedVector2Int : SharedVariable<Vector2Int>
    {
        public SharedVector2Int(Vector2Int value)
        {
            this.value = value;
        }
        public SharedVector2Int()
        {

        }
        protected override SharedVariable<Vector2Int> CloneT()
        {
            return new SharedVector2Int() { Value = value };
        }
    }
}