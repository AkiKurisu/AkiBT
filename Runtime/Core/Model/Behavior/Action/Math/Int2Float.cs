namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action: Convert int type to float type")]
    [AkiLabel("Math: Int2Float")]
    [AkiGroup("Math")]
    public class Int2Float : Action
    {
        public SharedInt value;
        [ForceShared]
        public SharedFloat newValue;
        protected override Status OnUpdate()
        {
            newValue.Value = value.Value;
            return Status.Success;
        }
    }
}
