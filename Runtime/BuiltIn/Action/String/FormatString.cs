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
        private string[] paramterValues;
        public override void Awake()
        {
            foreach (var value in parameters) InitVariable(value);
            paramterValues = new string[parameters.Count];
        }
        protected override Status OnUpdate()
        {
            for (int i = 0; i < paramterValues.Length; ++i)
            {
                paramterValues[i] = parameters[i].Value;
            }
            try
            {
                storeResult.Value = string.Format(format.Value, paramterValues);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
            return Status.Success;
        }
    }
}
