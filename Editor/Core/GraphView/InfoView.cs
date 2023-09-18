using UnityEngine.UIElements;
namespace Kurisu.AkiBT.Editor
{
    public class InfoView : VisualElement
    {
        public InfoView(string info)
        {
            Clear();
            IMGUIContainer container = new();
            container.Add(new Label(info));
            Add(container);
        }
        public void UpdateSelection(IBehaviorTreeNode node)
        {
            Clear();
            IMGUIContainer container = new();
            AkiInfoAttribute[] array;
            if ((array = node.GetBehavior().GetCustomAttributes(typeof(AkiInfoAttribute), false) as AkiInfoAttribute[]).Length > 0)
            {
                Label label = new(array[0].Description);
                container.Add(label);
            }
            Add(container);
        }
    }
}