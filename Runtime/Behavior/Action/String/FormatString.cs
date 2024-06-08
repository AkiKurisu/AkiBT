using System;
using System.Collections.Generic;
using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action: Format value of string")]
    [AkiLabel("String: Format")]
    [AkiGroup("String")]
    public class FormatString : Action
    {
        public SharedString format;
        public List<SharedString> parameters;
        [ForceShared]
        public SharedString storeResult;
        private string[] parameterValues;
        public override void Awake()
        {
            parameterValues = new string[parameters.Count];
        }
        protected override Status OnUpdate()
        {
            for (int i = 0; i < parameterValues.Length; ++i)
            {
                parameterValues[i] = parameters[i].Value;
            }
            try
            {
                storeResult.Value = string.Format(format.Value, parameterValues);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
            return Status.Success;
        }
    }
}
