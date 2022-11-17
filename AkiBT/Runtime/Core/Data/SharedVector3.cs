using System.Collections;
using System.Collections.Generic;
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
}
}