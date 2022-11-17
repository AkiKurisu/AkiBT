using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
}
}