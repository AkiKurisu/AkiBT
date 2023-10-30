using System.Reflection;
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
            AkiInfoAttribute infoAttribute;
            if ((infoAttribute = node.GetBehavior().GetCustomAttribute<AkiInfoAttribute>()) != null)
            {
                container.Add(new Label(infoAttribute.Description));
            }
            Add(container);
        }
    }
}