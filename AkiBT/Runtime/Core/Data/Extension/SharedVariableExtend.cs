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
    public static SharedVariable<T> GetValueFromTree<T>(this SharedVariable<T> variable,BehaviorTree tree)
    {
        if(!variable.IsShared)return variable;
        var value=tree.GetShareVariable<T>(variable.Name);
        if(value!=null)
            variable=value;
       return variable;
    }
}
}