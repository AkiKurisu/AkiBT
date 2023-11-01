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
        public override object Clone()
        {
            return new SharedVector3() { Value = value, Name = Name, IsShared = IsShared, IsGlobal = IsGlobal };
        }
    }
}