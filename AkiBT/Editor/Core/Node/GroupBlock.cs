using UnityEditor.Experimental.GraphView;
namespace Kurisu.AkiBT.Editor
{
    public class GroupBlock : Group
    {
       public GroupBlock()
       {
            capabilities |= Capabilities.Ascendable;
       } 
    }
}
