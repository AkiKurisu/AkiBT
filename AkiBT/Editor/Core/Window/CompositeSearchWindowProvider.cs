using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Kurisu.AkiBT.Editor
{
    public class CompositeSearchWindowProvider :CertainNodeSearchWindowProvider<Composite>
    {
        
        const string _nodeName="Composite";
        protected override string nodeName=>_nodeName;
    }
}