using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kurisu.AkiBT.Example
{
    public class ListTest : Action
    {
        [SerializeField]
        private List<SharedInt> sharedIntList=new List<SharedInt>();
        [SerializeField]
        private SharedFloat Float;
        protected override Status OnUpdate()
        {
            return Status.Success;
        }
    }
}
