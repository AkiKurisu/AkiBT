namespace Kurisu.AkiBT
{
    [System.Serializable]
    public class SharedInt : SharedVariable<int>, IBindableVariable<SharedInt>
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
            return new SharedInt() { Value = value, Name = Name, IsShared = IsShared };
        }

        public void Bind(SharedInt other)
        {
            base.Bind(other);
        }
    }
}