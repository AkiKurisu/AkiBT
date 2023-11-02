using System.Collections.Generic;
using UnityEngine;
namespace Kurisu.AkiBT
{
        public interface IBehaviorTree : IVariableSource
        {
#pragma warning disable IDE1006
                Object _Object { get; }
#pragma warning restore IDE1006
                Root Root
                {
                        get;
                }
#if UNITY_EDITOR
                /// <summary>
                /// Get block data from behavior tree graph, using only in editor
                /// </summary>
                /// <value></value>  
                List<GroupBlockData> BlockData { get; }
#endif
        }
}