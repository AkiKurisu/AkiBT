using System.Collections.Generic;
using UnityEngine;
namespace Kurisu.AkiBT
{
public interface IBehaviorTree 
{
    public Object _Object{get;}
    public Root Root{get;
        #if UNITY_EDITOR
            set;
        #endif
    }
    public List<SharedVariable> SharedVariables{get;
        #if UNITY_EDITOR
            set;
        #endif
    }
    #if UNITY_EDITOR
    public string SavePath{get;set;}
    public bool AutoSave{get;set;}
    /// <summary>
    /// Editor Only
    /// </summary>
    /// <value></value>
    public BehaviorTreeSO ExternalBehaviorTree{get;}  
    /// <summary>
    /// Editor Only
    /// </summary>
    /// <value></value>  
    public List<GroupBlockData> BlockData{get;set;}
    # endif
    /// <summary>
    /// 获取共享变量
    /// Get SharedVariable
    /// </summary>
    /// <param name="name">Variable Name</param>
    /// <typeparam name="T">Variabe Type</typeparam>
    /// <returns></returns>
    SharedVariable<T> GetShareVariable<T>(string name); 
}
}