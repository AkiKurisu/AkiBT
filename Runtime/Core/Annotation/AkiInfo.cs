using System;

namespace Kurisu.AkiBT
{
    /// <summary>
    /// 在编辑器中描述结点行为
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class AkiInfo : Attribute
    {
        public string Description
		{
			get
			{
				return this.mDescription;
			}
		}

		private readonly string mDescription;
        public AkiInfo(string description)
        {
            this.mDescription=description;
        }
    }
}