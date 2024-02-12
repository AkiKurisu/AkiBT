namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action: Replace value of string")]
    [AkiLabel("String: Replace")]
    [AkiGroup("String")]
    public class ReplaceString : Action
    {
        public SharedString target;
        public SharedString replaceFrom;
        public SharedString replaceTo;
        [ForceShared]
        public SharedString storeResult;
        protected override Status OnUpdate()
        {
            storeResult.Value = target.Value.Replace(replaceFrom.Value, replaceTo.Value);
            return Status.Success;
        }
    }
}
