using System;
using UnityEngine;
namespace Kurisu.AkiBT
{
    [Serializable]
    public class SharedObject : SharedVariable<UnityEngine.Object>, IBindableVariable<SharedObject>
    {
#if UNITY_EDITOR
        [SerializeField]
        private string constraintTypeAQM;
        public string ConstraintTypeAQM { get => constraintTypeAQM; set => constraintTypeAQM = value; }
#endif
        public SharedObject(UnityEngine.Object value)
        {
            this.value = value;
        }
        public SharedObject()
        {

        }
        public override object Clone()
        {
            return new SharedObject() { Value = value, Name = Name, IsShared = IsShared, ConstraintTypeAQM = ConstraintTypeAQM };
        }

        public void Bind(SharedObject other)
        {
            base.Bind(other);
        }
    }
    [Serializable]
    public class SharedTObject<TObject> : SharedVariable<TObject>, IBindableVariable<SharedObject>, IBindableVariable<SharedTObject<TObject>> where TObject : UnityEngine.Object
    {
        public SharedTObject(TObject value)
        {
            this.value = value;
        }
        public SharedTObject()
        {

        }
        public override object Clone()
        {
            return new SharedTObject<TObject>() { Value = value, Name = Name, IsShared = IsShared };
        }

        public void Bind(SharedObject other)
        {
            Getter = () => (TObject)other.Value;
            Setter = (evt) => other.Value = evt;
        }

        public void Bind(SharedTObject<TObject> other)
        {
            base.Bind(other);
        }
    }
}
