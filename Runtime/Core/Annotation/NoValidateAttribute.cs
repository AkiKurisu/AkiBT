using System;
namespace Kurisu.AkiBT
{
    /// <summary>
    /// Skip Composite check port legitimacy, allow the port not to connect to the node after use
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class NoValidateAttribute : Attribute
    {

    }
}
