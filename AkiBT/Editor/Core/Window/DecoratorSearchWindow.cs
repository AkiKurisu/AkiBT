using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Kurisu.AkiBT.Editor
{
    public class DecortaorSearchWindowProvider :CertainNodeSearchWindowProvider<Decorator>
    {
        const string _nodeName="Decorator";
        protected override string nodeName=>_nodeName;
    }
}