using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action : Fixed return value," +
     " returns a fixed value after running, you can put the node at the end of the combination logic to keep the return value.")]
    [AkiLabel("Logic : FixedReturn")]
    [AkiGroup("Logic")]
    public class FixedReturn : Action
    {
        [SerializeField]
        private Status returnStatus;
        protected override Status OnUpdate()
        {
            return returnStatus;
        }
    }
}