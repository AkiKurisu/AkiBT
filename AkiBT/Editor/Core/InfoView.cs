using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
namespace Kurisu.AkiBT.Editor
{
public class InfoView : VisualElement
{
    public InfoView()
    {
        Clear();
        IMGUIContainer container=new IMGUIContainer();
        container.Add(new Label($"欢迎使用AkiBT,一个超简单的行为树!"));
        Add(container);
    }
    public void UpdateSelection(BehaviorTreeNode node)
    {
        Clear();
        IMGUIContainer container=new IMGUIContainer();
        AkiInfo[] array;
        if ((array = (node.GetBehavior().GetCustomAttributes(typeof(AkiInfo), false) as AkiInfo[])).Length > 0)
        {
            Label label=new Label(array[0].Description);
            container.Add(label);
        }
        Add(container);
    }
   
}
}