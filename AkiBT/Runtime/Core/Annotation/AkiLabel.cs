using System;

namespace Kurisu.AkiBT
{
    /// <summary>
    /// 对行为结点在编辑器中的名称进行替换,也可以对字段名称进行替换
    /// </summary>
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class AkiLabel : Attribute
    {
        public string Title
		{
			get
			{
				return this.mTitle;
			}
		}

		private readonly string mTitle;
        public AkiLabel(string tite)
        {
            this.mTitle=tite;
        }
    }

}