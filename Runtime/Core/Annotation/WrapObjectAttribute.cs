using System;
namespace Kurisu.AkiBT
{
    /// <summary>
    /// Wrap object to use legacy IMGUI property field
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class WrapObjectAttribute : Attribute
    {

    }
}
