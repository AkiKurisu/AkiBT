using System;
namespace Kurisu.AkiBT
{
    [Serializable]
    public class SharedFloat : SharedVariable<float>, IBindableVariable<SharedFloat>
    {
        public SharedFloat(float value)
        {
            this.value = value;
        }
        public SharedFloat()
        {

        }
        public override object Clone()
        {
            return new SharedFloat() { Value = value, Name = Name, IsShared = IsShared };
        }
        public void Bind(SharedFloat other)
        {
            base.Bind(other);
        }
    }
}