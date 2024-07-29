namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action: Flip bool value")]
    [AkiLabel("Math: BoolFlip")]
    [AkiGroup("Math")]
    public class BoolFlip : Action
    {
        [ForceShared]
        public SharedBool boolToFlip;
        protected override Status OnUpdate()
        {
            boolToFlip.Value = !boolToFlip.Value;
            return Status.Success;
        }
    }
}