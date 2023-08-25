using System;
namespace Kurisu.AkiBT
{
    /// <summary>
    /// Nodes are categorized in the editor dropdown menu, and can be sub-categorized with the '/' symbol
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class AkiGroupAttribute : Attribute
    {
        public string Group
        {
            get
            {
                return this.mGroup;
            }
        }

        private readonly string mGroup;
        public AkiGroupAttribute(string group)
        {
            this.mGroup = group;
        }
    }

}