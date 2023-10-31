using System.Collections.Generic;
namespace Kurisu.AkiBT
{
    /// <summary>
    /// Shared variables owner
    /// </summary>
    public interface IVariableSource
    {
        List<SharedVariable> SharedVariables { get; }
    }
    public interface IVariableScope
    {
        /// <summary>
        /// Scope based global variables
        /// </summary>
        GlobalVariables GlobalVariables { get; }
    }
}
