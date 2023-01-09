using UnityEngine;
namespace Kurisu.AkiBT
{
    [System.Serializable]
public class SharedVector3 : SharedVariable<Vector3>
{
    public SharedVector3(Vector3 vector3)
    {
        this.value=vector3;
    }
    public SharedVector3()
    {
        
    }
    public override object Clone()
    {
        return new SharedVector3(){Value=this.value,Name=this.Name,IsShared=this.IsShared};
    }
}
}