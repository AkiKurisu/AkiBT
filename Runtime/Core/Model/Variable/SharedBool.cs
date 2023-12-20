using System;
namespace Kurisu.AkiBT
{
    [Serializable]
    public class SharedBool : SharedVariable<bool>
    {
        public SharedBool(bool value)
        {
            this.value = value;
        }
        public SharedBool()
        {

        }
        public override SharedVariable Clone()
        {
            return new SharedBool() { Value = value, Name = Name, IsShared = IsShared, IsGlobal = IsGlobal };
        }
    }
}