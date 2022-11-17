using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
}
}