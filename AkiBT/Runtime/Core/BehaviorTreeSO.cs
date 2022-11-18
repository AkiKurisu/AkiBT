using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Kurisu.AkiBT
{
public class BehaviorTreeSO : ScriptableObject
{
    [SerializeReference,Header("你可以使用SO文件保存你的行为树版本")]
    private Root root = new Root();
    public Root Root=>root;
   
    public void SetRoot(Root newRoot)
    {
        this.root=newRoot;
    }
}
}