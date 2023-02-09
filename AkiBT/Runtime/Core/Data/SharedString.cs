namespace Kurisu.AkiBT
{
    [System.Serializable]
    public class SharedString : SharedVariable<string>
    {
        public SharedString(string value)
        {
            this.value=value;
        }
        public SharedString()
        {
            
        }
        public override object Clone()
        {
            return new SharedString(){Value=this.value,Name=this.Name,IsShared=this.IsShared};
        }
    }
}
