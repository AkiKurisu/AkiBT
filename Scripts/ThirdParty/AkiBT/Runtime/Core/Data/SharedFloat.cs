namespace Kurisu.AkiBT
{
    [System.Serializable]
public class SharedFloat : SharedVariable<float>
{
    public SharedFloat(float value)
    {
        this.value=value;
    }
   public SharedFloat()
    {
        
    }
    public override object Clone()
    {
        return new SharedFloat(){Value=this.value,Name=this.Name,IsShared=this.IsShared};
    }
}
}