using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using System.Reflection;
using System;
using Kurisu.AkiBT;
using UnityEditor.UIElements;
using Kurisu.AkiBT.Editor;

namespace Kurisu.AkiST.Editor
{
    public class SkillTreeView : BehaviorTreeView
    {
        public SkillTreeView(IBehaviorTree bt, EditorWindow editor) : base(bt, editor)
        {
        }
        protected override string treeEditorName=>"AkiST";
    }
}