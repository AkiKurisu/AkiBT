using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Kurisu.AkiBT
{
public static class SharedVariableExtend
{
    /// <summary>
    /// 从行为树获取共享变量
    /// </summary>
    /// <param name="variable"></param>
    /// <param name="tree"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static SharedVariable<T> GetValueFromTree<T>(this SharedVariable<T> variable,IBehaviorTree tree)
    {
        if(variable==null)
        {
            Debug.LogWarning("你没有实例SharedVarible!");
            return null;
        }
        if(!variable.IsShared)return variable;
        var value=tree.GetShareVariable<T>(variable.Name);
        if(value!=null)
        {
            variable.Bind(value);
        }
        else
            Debug.Log("获取了空值");
       return variable;
    }
}
}