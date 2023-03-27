using System.Collections.Generic;
using UnityEngine;
namespace Kurisu.AkiBT
{
/// <summary>
/// Behavior Tree ScriptableObject, can only run manually.
/// </summary>
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
    [Multiline,SerializeField]
    public string Description;
    #endif
    [HideInInspector]
    [SerializeReference]
    protected List<SharedVariable> sharedVariables = new List<SharedVariable>();
    SharedVariable<T> IBehaviorTree.GetShareVariable<T>(string name)
    {
        if(string.IsNullOrEmpty(name))
        {
            Debug.LogError($"共享变量名称不能为空",this);
            return null;
        }
        foreach(var variable in sharedVariables)
        {
            if(variable.Name.Equals(name))
            {
                if( variable is SharedVariable<T>)return variable as SharedVariable<T>;
                else Debug.LogError($"{name}名称变量不是{typeof(T).Name}类型",this);
                return null;
            }
        }
        Debug.LogError($"没有找到共享变量:{name}",this);
        return null;
    }
    /// <summary>
    /// 外部传入绑定对象并初始化,调用Awake和Start方法
    /// Bind GameObject and Init behaviorTree through Awake and Start method
    /// </summary>
    /// <param name="gameObject"></param>
    public void Init(GameObject gameObject) {
        root.Run(gameObject,this);
        root.Awake();
        root.Start();
    } 
    /// <summary>
    /// 外部调用Update更新,此方法运行完全基于SO
    /// Update BehaviorTree externally, this method is completely based on ScriptableObject
    /// </summary>
    public virtual void Update()
    {
        root.PreUpdate();
        root.Update();
        root.PostUpdate();
    }
}
}