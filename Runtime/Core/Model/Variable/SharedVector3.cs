using System;
using UnityEngine;
namespace Kurisu.AkiBT
{
    [Serializable]
    public class SharedVector3 : SharedVariable<Vector3>
    {
        public SharedVector3(Vector3 value)
        {
            this.value = value;
        }
        public SharedVector3()
        {

        }
        protected override SharedVariable<Vector3> CloneT()
        {
            return new SharedVector3() { Value = value };
        }
    }
    [Serializable]
    public class SharedVector3Int : SharedVariable<Vector3Int>
    {
        public SharedVector3Int(Vector3Int value)
        {
            this.value = value;
        }
        public SharedVector3Int()
        {

        }
        protected override SharedVariable<Vector3Int> CloneT()
        {
            return new SharedVector3Int() { Value = value };
        }
    }
}