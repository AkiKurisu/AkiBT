using UnityEngine.UIElements;
namespace Kurisu.AkiBT.Editor
{
public class InfoView : VisualElement
{
    public InfoView(string info)
    {
        Clear();
        IMGUIContainer container=new IMGUIContainer();
        container.Add(new Label(info));
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