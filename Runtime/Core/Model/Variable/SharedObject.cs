using System;
using UnityEngine;
using UObject = UnityEngine.Object;
namespace Kurisu.AkiBT
{
    [Serializable]
    public class SharedObject : SharedVariable<UObject>
    {
        [SerializeField]
        private string constraintTypeAQM;
        public string ConstraintTypeAQM { get => constraintTypeAQM; set => constraintTypeAQM = value; }
        public SharedObject(UObject value)
        {
            this.value = value;
        }
        public SharedObject()
        {

        }
        protected override SharedVariable<UObject> CloneT()
        {
            return new SharedObject() { Value = value, ConstraintTypeAQM = constraintTypeAQM };
        }
        public SharedTObject<TObject> ConvertT<TObject>() where TObject : UObject
        {
            var clone = new SharedTObject<TObject>() { Value = (TObject)value };
            clone.CopyProperty(this);
            return clone;
        }
    }
    [Serializable]
    public class SharedTObject<TObject> : SharedVariable<TObject>, IBindableVariable<UObject> where TObject : UObject
    {
        //Special case of binding SharedTObject<T> to SharedObject
        UObject IBindableVariable<UObject>.Value
        {
            get
            {
                return Value;
            }

            set
            {
                Value = (TObject)value;
            }
        }

        public SharedTObject(TObject value)
        {
            this.value = value;
        }
        public SharedTObject()
        {

        }
        protected override SharedVariable<TObject> CloneT()
        {
            return new SharedTObject<TObject>() { Value = value };
        }
        public override void Bind(SharedVariable other)
        {
            //Special case of binding SharedObject to SharedTObject<T>
            if (other is IBindableVariable<UObject> sharedObject)
            {
                Bind(sharedObject);
            }
            else
            {
                base.Bind(other);
            }
        }
        public void Bind(IBindableVariable<UObject> other)
        {
            Getter = () => { return (TObject)other.Value; };
            Setter = (evt) => other.Value = evt;
        }
        public SharedObject Convert()
        {
            var clone = new SharedObject() { Value = value };
            clone.CopyProperty(this);
            return clone;
        }
    }
}
