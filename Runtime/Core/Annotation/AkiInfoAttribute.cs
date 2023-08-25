using System;
namespace Kurisu.AkiBT
{
    /// <summary>
    /// Describe node behavior in the editor
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class AkiInfoAttribute : Attribute
    {
        public string Description
        {
            get
            {
                return mDescription;
            }
        }

        private readonly string mDescription;
        public AkiInfoAttribute(string description)
        {
            mDescription = description;
        }
    }
}