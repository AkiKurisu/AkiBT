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
    #if UNITY_EDITOR
    [SerializeField,HideInInspector]
    private bool autoSave;
    [SerializeField,HideInInspector]
    private string savePath="Assets";
    public string SavePath
    {
            get => savePath;
            set => savePath = value;
    }
    public bool AutoSave
    {
        get => autoSave;
        set => autoSave = value;
    }
    public virtual BehaviorTreeSO ExternalBehaviorTree=>null;
    [SerializeField,HideInInspector]
    private List<GroupBlockData> blockData=new List<GroupBlockData>();
    public List<GroupBlockData> BlockData { get => blockData; set=>blockData=value;}
    #endif
    [HideInInspector]
    [SerializeReference]
    private List<SharedVariable> sharedVariables = new List<SharedVariable>();
    protected Dictionary<string,int>sharedVariableIndex;
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
    #if UNITY_EDITOR
    void OnValidate()
    {
        sharedVariableIndex=new Dictionary<string, int>();
        for(int i=0;i<this.SharedVariables.Count;i++)
        {
            sharedVariableIndex.Add(this.SharedVariables[i].Name,i);
        }
    }
   #else
    void Awake()
    {
        sharedVariableIndex=new Dictionary<string, int>();
        for(int i=0;i<this.SharedVariables.Count;i++)
        {
            sharedVariableIndex.Add(this.SharedVariables[i].Name,i);
        }
    }
   #endif
    /// <summary>
    /// 外部传入绑定对象并初始化
    /// </summary>
    /// <param name="gameObject"></param>
    public void Init(GameObject gameObject) {
        root.Run(gameObject,this);
        root.Awake();
        root.Start();
    } 
    /// <summary>
    /// 外部调用Update更新,此方法运行完全基于SO
    /// </summary>
    public virtual void Update()
    {
        root.PreUpdate();
        root.Update();
        root.PostUpdate();
    }
}
}