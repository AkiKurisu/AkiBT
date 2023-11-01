using System;
namespace Kurisu.AkiBT
{
    [Serializable]
    public class SharedInt : SharedVariable<int>
    {
        public SharedInt(int value)
        {
            this.value = value;
        }
        public SharedInt()
        {

        }
        public override object Clone()
        {
            return new SharedInt() { Value = value, Name = Name, IsShared = IsShared, IsGlobal = IsGlobal };
        }
    }
}