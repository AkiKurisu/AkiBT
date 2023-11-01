using UnityEngine.UIElements;
namespace Kurisu.AkiBT
{
    public enum VariableChangeType
    {
        Create,
        ValueChange,
        NameChange,
        Delate
    }
    public class VariableChangeEvent : EventBase<VariableChangeEvent>
    {
        public SharedVariable Variable { get; protected set; }
        public VariableChangeType ChangeType { get; protected set; }
        public static VariableChangeEvent GetPooled(SharedVariable notifyVariable, VariableChangeType changeType)
        {
            VariableChangeEvent changeEvent = GetPooled();
            changeEvent.Variable = notifyVariable;
            changeEvent.ChangeType = changeType;
            return changeEvent;
        }
    }
}
