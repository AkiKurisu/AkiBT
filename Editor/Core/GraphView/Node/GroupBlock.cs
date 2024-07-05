using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
namespace Kurisu.AkiBT.Editor
{
    public class GroupBlock : Group
    {
        public GroupBlock()
        {
            this.AddManipulator(new ContextualMenuManipulator(BuildContextualMenu));
            capabilities |= Capabilities.Ascendable;
        }
        private void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.MenuItems().Clear();
            evt.menu.MenuItems().Add(new BehaviorTreeDropdownMenuAction("Unselect All", (a) =>
            {
                //Clone to prevent self modify
                RemoveElements(containedElements.ToArray());
            }));
        }
        public void Commit(List<GroupBlockData> blockData)
        {
            var nodes = containedElements
                                .OfType<IBehaviorTreeNode>()
                                .Select(x => x.Guid).ToList();
            blockData.Add(new GroupBlockData
            {
                ChildNodes = nodes,
                Title = title,
                Position = GetPosition().position
            });
        }
    }
}
