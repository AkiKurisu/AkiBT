using System;
namespace Kurisu.AkiBT
{
    [Serializable]
    public class SharedFloat : SharedVariable<float>
    {
        public SharedFloat(float value)
        {
            this.value = value;
        }
        public SharedFloat()
        {

        }
        public override SharedVariable Clone()
        {
            return new SharedFloat() { Value = value, Name = Name, IsShared = IsShared, IsGlobal = IsGlobal };
        }
    }
}