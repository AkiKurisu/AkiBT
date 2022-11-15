using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Kurisu.AkiBT.Editor
{
    public class ActionSearchWindowProvider : CertainNodeSearchWindowProvider<Action>
    {
        const string _nodeName="Action";
        protected override string nodeName=>_nodeName;
    }
}