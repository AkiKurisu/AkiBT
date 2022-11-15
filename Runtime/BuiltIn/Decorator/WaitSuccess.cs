using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Decorator:直到子结点返回Success为止返回Running")]
    [AkiLabel("WaitSuccess等待正确")]
public class WaitSuccess : Decorator
{
    protected override Status OnDecorate(Status childeStatus)
    {
        if(childeStatus==Status.Success)
            return Status.Success;
        else
            return Status.Running;
    }
}
}