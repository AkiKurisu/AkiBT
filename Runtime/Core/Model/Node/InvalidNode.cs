using UnityEngine;
namespace Kurisu.AkiBT
{
    [AkiGroup("Hidden")]
    [AkiLabel("<color=#FFE000><b>Class Missing!</b></color>")]
    [AkiInfo("The presence of this node indicates that the namespace, class name, or assembly of the behavior may be changed.")]
    internal sealed class InvalidAction : Action
    {
        [Multiline]
        public string nodeType;
        [Multiline]
        public string serializedData;
        protected override Status OnUpdate()
        {
            return Status.Success;
        }
    }
    [AkiGroup("Hidden")]
    [AkiLabel("<color=#FFE000><b>Class Missing!</b></color>")]
    [AkiInfo("The presence of this node indicates that the namespace, class name, or assembly of the behavior may be changed.")]
    internal sealed class InvalidComposite : Composite
    {
        [Multiline]
        public string nodeType;
        [Multiline]
        public string serializedData;
        protected override Status OnUpdate()
        {
            return Status.Success;
        }
    }
}