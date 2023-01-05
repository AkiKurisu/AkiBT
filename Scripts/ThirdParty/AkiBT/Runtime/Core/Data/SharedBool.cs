using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
}
}