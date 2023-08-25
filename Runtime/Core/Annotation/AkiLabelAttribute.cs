using System;
namespace Kurisu.AkiBT
{
    /// <summary>
    /// Replace the name of the behavior node in the editor, or replace the field name
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class AkiLabelAttribute : Attribute
    {
        public string Title
        {
            get
            {
                return mTitle;
            }
        }

        private readonly string mTitle;
        public AkiLabelAttribute(string tite)
        {
            mTitle = tite;
        }
    }

}