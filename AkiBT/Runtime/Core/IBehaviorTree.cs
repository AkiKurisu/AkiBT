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
    public BehaviorTreeSO ExternalBehaviorTree{get;}    
    public List<GroupBlockData> BlockData{get;set;}
    # endif
    SharedVariable<T> GetShareVariable<T>(string name);
    
    
}
}