namespace Kurisu.AkiBT
{
    [System.Serializable]
public class SharedInt : SharedVariable<int>
{
    public SharedInt(int value)
    {
        this.value=value;
    }
    public SharedInt()
    {
        
    }
    public override object Clone()
    {
        return new SharedInt(){Value=this.value,Name=this.Name,IsShared=this.IsShared};
    }
}
}