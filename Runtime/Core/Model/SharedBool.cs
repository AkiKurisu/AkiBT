namespace Kurisu.AkiBT
{
    [System.Serializable]
    public class SharedBool : SharedVariable<bool>, IBindableVariable<SharedBool>
    {
        public SharedBool(bool value)
        {
            this.value = value;
        }
        public SharedBool()
        {

        }
        public override object Clone()
        {
            return new SharedBool() { Value = value, Name = Name, IsShared = IsShared };
        }

        public void Bind(SharedBool other)
        {
            base.Bind(other);
        }
    }
}