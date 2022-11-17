using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Kurisu.AkiBT.Editor
{
    public class ConditionalSearchWindowProvider : CertainNodeSearchWindowProvider<Conditional>
    {
        const string _nodeName="Conditional";
        protected override string nodeName=>_nodeName;
    }
}