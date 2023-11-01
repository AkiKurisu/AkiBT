using System;
namespace Kurisu.AkiBT
{
    /// <summary>
    /// Wrap field to use legacy IMGUI property field
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class WrapFieldAttribute : Attribute
    {

    }
}
