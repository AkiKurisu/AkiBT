using System;

namespace Kurisu.AkiBT
{
    /// <summary>
    /// 行为结点在编辑器中的名称进行替换
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
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