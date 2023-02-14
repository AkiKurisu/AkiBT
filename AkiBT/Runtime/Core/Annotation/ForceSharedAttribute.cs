using System;
namespace Kurisu.AkiBT
{
    /// <summary>
    /// 强制编辑器中SharedVariable开启共享,例如storeResult字段应该始终开启共享
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class ForceSharedAttribute : Attribute
    {

    }
}
