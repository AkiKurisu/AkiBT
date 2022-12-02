using System.Collections;
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
    public string SavePath{get;
    #if UNITY_EDITOR
    set;
    #endif
    }
    public bool AutoSave{get;
    #if UNITY_EDITOR
    set;
    #endif
    }
    SharedVariable<T> GetShareVariable<T>(string name);
    public BehaviorTreeSO ExternalBehaviorTree{get;}    
}
}