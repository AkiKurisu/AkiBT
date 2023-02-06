using System;

namespace Kurisu.AkiBT
{
    /// <summary>
    /// 行为结点在编辑器下拉菜单中进行分类,可以用'/'符号进行子分类
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class AkiGroup : Attribute
    {
        public string Group
		{
			get
			{
				return this.mGroup;
			}
		}

		private readonly string mGroup;
        public AkiGroup(string group)
        {
            this.mGroup=group;
        }

	
    }

}