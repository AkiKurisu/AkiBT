using System;

namespace Kurisu.AkiBT
{
    /// <summary>
    /// 在编辑器中添加下拉容器FolderOut
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class AkiFolder : Attribute
    {
        public string Folder
		{
			get
			{
				return this.mFolder;
			}
		}

		private readonly string mFolder;
        public AkiFolder(string folder)
        {
            this.mFolder=folder;
        }

	
    }

}