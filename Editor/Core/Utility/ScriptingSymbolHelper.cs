using System.Linq;
using UnityEditor;
using UnityEditor.Build;
namespace Kurisu.AkiBT.Editor
{
    public static class ScriptingSymbolHelper
    {
        private static NamedBuildTarget GetActiveNamedBuildTarget()
        {
            var buildTargetGroup = GetActiveBuildTargetGroup();
            var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup);

            return namedBuildTarget;
        }

        private static BuildTargetGroup GetActiveBuildTargetGroup()
        {
            var buildTarget = EditorUserBuildSettings.activeBuildTarget;
            var buildTargetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);

            return buildTargetGroup;
        }

        public static void AddScriptingSymbol(string define)
        {
            var namedBuildTarget = GetActiveNamedBuildTarget();
            PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget, out var defines);

            var defineList = defines.ToList();

            if (!defineList.Contains(define))
            {
                defineList.Add(define);
            }
            PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, defineList.ToArray());
        }
        public static bool ContainsScriptingSymbol(string define)
        {
            var namedBuildTarget = GetActiveNamedBuildTarget();
            PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget, out var defines);
            var defineList = defines.ToList();
            return defineList.Contains(define);
        }
        public static void RemoveScriptingSymbol(string define)
        {
            var namedBuildTarget = GetActiveNamedBuildTarget();
            PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget, out var defines);

            var defineList = defines.ToList();

            if (defineList.Contains(define))
            {
                defineList.Remove(define);
            }
            PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, defineList.ToArray());
        }
    }
}