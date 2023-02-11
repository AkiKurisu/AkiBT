using System;

namespace Kurisu.AkiST
{
    /// <summary>
    /// 在编辑器中添加下拉容器FolderOut
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class AkiFolderAttribute : Attribute
    {
        public string Folder
		{
			get
			{
				return this.mFolder;
			}
		}

		private readonly string mFolder;
        public AkiFolderAttribute(string folder)
        {
            this.mFolder=folder;
        }

	
    }

}