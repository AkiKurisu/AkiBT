using System;
namespace Kurisu.AkiBT
{
    /// <summary>
    /// Disable copying of nodes within the editor
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class CopyDisableAttribute : Attribute
    {

    }
}
