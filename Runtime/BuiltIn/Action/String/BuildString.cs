using System.Collections.Generic;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action: Build value of string")]
    [AkiLabel("String: Build")]
    [AkiGroup("String")]
    public class BuildString : Action
    {
        public List<SharedString> values;
        [ForceShared]
        public SharedString storeResult;
        public override void Awake()
        {
            foreach (var value in values) InitVariable(value);
        }
        protected override Status OnUpdate()
        {
            for (int i = 0; i < values.Count; i++)
            {
                storeResult.Value += values[i].Value;
            }
            return Status.Success;
        }
    }
}
