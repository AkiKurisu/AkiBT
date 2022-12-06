using System.Collections.Generic;
using UnityEngine;
using Kurisu.AkiBT;
namespace Kurisu.AkiST
{
public abstract class SkillTreeSO : BehaviorTreeSO
{
    public virtual bool UseTree=>true;
    [SerializeField,HideInInspector]
    private SkillTreeSO externalSkill;
    [Multiline,SerializeField,AkiLabel("技能描述")]
    protected string description;
    public string Description=>description;
    public override BehaviorTreeSO ExternalBehaviorTree=>externalSkill;
    public event System.Action OnSkillExitEvent;
    protected void Init(GameObject gameObject) {
        sharedVariableIndex=new Dictionary<string, int>();
        for(int i=0;i<this.SharedVariables.Count;i++)
        {
            sharedVariableIndex.Add(this.SharedVariables[i].Name,i);
        }
        root.Run(gameObject,this);
        root.Awake();
        root.Start();
    } 
    protected void Tick()
    {
        root.PreUpdate();
        root.Update();
        root.PostUpdate();
    }
    public void OnSkillExit()
   {
      OnSkillExitEvent?.Invoke();
   }
    /// <summary>
    /// 处理技能进入=>需要初始化Init()传入绑定对象
    /// </summary>
    /// <param name="model"></param>
    protected abstract void OnSkillEnter();
    /// <summary>
    /// 处理技能更新=>调用Tick()
    /// </summary>
    public virtual void OnSkillUpdate()
    {
        Tick();
    }
}
}