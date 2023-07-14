using System.Collections.Generic;
using UnityEngine;
namespace Kurisu.AkiBT
{
    public interface IBehaviorTree 
    {
        Object _Object{get;}
        Root Root{get;
            #if UNITY_EDITOR
                set;
            #endif
        }
        List<SharedVariable> SharedVariables{get;
            #if UNITY_EDITOR
                set;
            #endif
        }
        #if UNITY_EDITOR
        /// <summary>
        /// Get external behavior tree, using only in editor
        /// </summary>
        /// <value></value>
        BehaviorTreeSO ExternalBehaviorTree{get;}  
        /// <summary>
        /// Get block data from behavior tree graph, using only in editor
        /// </summary>
        /// <value></value>  
        List<GroupBlockData> BlockData{get;set;}
        # endif
        /// <summary>
        /// Get shared variable by its name
        /// Get SharedVariable
        /// </summary>
        /// <param name="name">Variable Name</param>
        /// <typeparam name="T">Variabe Type</typeparam>
        /// <returns></returns>
        SharedVariable<T> GetShareVariable<T>(string name); 
    }
}