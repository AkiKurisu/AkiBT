namespace Kurisu.AkiBT
{
    [System.Serializable]
    public class SharedString : SharedVariable<string>, IBindableVariable<SharedString>
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
            return new SharedString() { Value = value, Name = Name, IsShared = IsShared };
        }

        public void Bind(SharedString other)
        {
            base.Bind(other);
        }
    }
}
