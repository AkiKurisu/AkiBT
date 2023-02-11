using System;
namespace Kurisu.AkiBT
{
    /// <summary>
    /// 跳过Composite检查端口合法性,使用后允许端口不连接结点
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class NoValidateAttribute : Attribute
    {

    }
}
