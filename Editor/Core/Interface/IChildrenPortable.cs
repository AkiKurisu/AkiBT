using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
namespace Kurisu.AkiBT.Editor
{
    public interface IChildPortable
    {
        Port Child { get; }
    }
}
