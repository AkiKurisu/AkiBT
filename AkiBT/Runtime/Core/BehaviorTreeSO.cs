using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Kurisu.AkiBT
{
[CreateAssetMenu(fileName = "BehaviorTreeSO", menuName = "AkiBT/BehaviorTreeSO")]
public class BehaviorTreeSO : ScriptableObject,IBehaviorTree
{
    [SerializeReference,HideInInspector]
    protected Root root = new Root();
    Object IBehaviorTree._Object=>this;
    public Root Root
    {
        get=>root;
        #if UNITY_EDITOR
            set => root = value;
        #endif
    }
    public List<SharedVariable> SharedVariables
    {
        get=>sharedVariables;
        #if UNITY_EDITOR
            set=>sharedVariables=value;
        #endif
    }

    [SerializeField,HideInInspector]
    private bool autoSave;
    [SerializeField,HideInInspector]
    private string savePath="Assets";
    public string SavePath
    {
            get => savePath;
    #if UNITY_EDITOR
            set => savePath = value;
    #endif
    }
    public bool AutoSave
    {

        get => autoSave;
#if UNITY_EDITOR
        set => autoSave = value;
#endif
    }
    [HideInInspector]
    [SerializeReference]
    private List<SharedVariable> sharedVariables = new List<SharedVariable>();
    protected Dictionary<string,int>sharedVariableIndex;
    public virtual BehaviorTreeSO ExternalBehaviorTree=>null;
    /// <summary>
    /// 获取共享变量
    /// </summary>
    /// <param name="name"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
     SharedVariable<T> IBehaviorTree.GetShareVariable<T>(string name)
    {
        if(string.IsNullOrEmpty(name))return null;
        int index;
        if(sharedVariableIndex.TryGetValue(name,out index))
        {
            var variable=sharedVariables[index];
            if(variable is SharedVariable<T>)
            {
                return variable as SharedVariable<T>;
            }
            else
            {
                Debug.LogError($"{name}名称变量不是{typeof(T).Name}类型");
            }
        }
        else
        {
            Debug.LogError($"没有在行为树中找到共享变量:{name}");
        }
        return null;
    }
}
}