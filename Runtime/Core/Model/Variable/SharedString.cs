using System;
namespace Kurisu.AkiBT
{
    [Serializable]
    public class SharedString : SharedVariable<string>
    {
        public SharedString(string value)
        {
            this.value = value;
        }
        public SharedString()
        {

        }
        public override object Clone()
        {
            return new SharedString() { Value = value, Name = Name, IsShared = IsShared, IsGlobal = IsGlobal };
        }
    }
}
