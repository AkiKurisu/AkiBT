using System;
using System.Collections.Generic;
#if UNITY_2022_1_OR_NEWER
using UnityEngine.UIElements;
#else
using UnityEditor.UIElements;
#endif
namespace Kurisu.AkiBT.Editor
{
    public class EnumField : PopupField<Enum>
    {
        public EnumField(string label, List<Enum> choices, Enum defaultValue = null)
            : base(label, choices, defaultValue, null, null)
        {
        }
    }
}