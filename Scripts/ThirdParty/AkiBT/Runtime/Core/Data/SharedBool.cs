namespace Kurisu.AkiBT
{
    [System.Serializable]
public class SharedBool : SharedVariable<bool>
{
    public SharedBool(bool value)
    {
        this.value=value;
    }
    public SharedBool()
    {
        
    }
    public override object Clone()
    {
        return new SharedBool(){Value=this.value,Name=this.Name,IsShared=this.IsShared};
    }
    }
}