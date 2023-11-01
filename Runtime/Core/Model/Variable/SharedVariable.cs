using UnityEngine;
using System;
namespace Kurisu.AkiBT
{
	[Serializable]
	public abstract class SharedVariable : ICloneable
	{
		public SharedVariable()
		{

		}
		/// <summary>
		/// Whether variable is shared
		/// </summary>
		/// <value></value>
		public bool IsShared
		{
			get => isShared;
			set => isShared = value;
		}
		[SerializeField]
		private bool isShared;
		/// <summary>
		/// Whether variable is global
		/// </summary>
		/// <value></value>
		public bool IsGlobal
		{
			get => isGlobal;
			set => isGlobal = value;
		}
		[SerializeField]
		private bool isGlobal;
		public string Name
		{
			get
			{
				return mName;
			}
			set
			{
				mName = value;
			}
		}
		public abstract object GetValue();
		public abstract void SetValue(object value);
		/// <summary>
		/// Bind to other sharedVariable
		/// </summary>
		/// <param name="other"></param>
		public abstract void Bind(SharedVariable other);
		/// <summary>
		/// Unbind self
		/// </summary>
		public abstract void Unbind();
		/// <summary>
		/// Clone shared variable by deep copy, an option here is to override for preventing using reflection
		/// </summary>
		/// <returns></returns>
		public virtual object Clone()
		{
			return ReflectionHelper.DeepCopy(this);
		}

		[SerializeField]
		private string mName;
		public abstract ObserveVariable Observe();
	}
	[Serializable]
	public abstract class SharedVariable<T> : SharedVariable, IBindableVariable<T>
	{
		public T Value
		{
			get
			{
				return (Getter == null) ? value : Getter();
			}
			set
			{
				if (Setter != null)
				{
					Setter(value);
				}
				else
				{
					this.value = value;
				}
			}
		}
		public sealed override object GetValue()
		{
			return Value;
		}
		public sealed override void SetValue(object value)
		{
			if (Setter != null)
			{
				Setter((T)value);
			}
			else if (value is IConvertible)
			{
				this.value = (T)Convert.ChangeType(value, typeof(T));
			}
			else
			{
				this.value = (T)value;
			}
		}
		protected Func<T> Getter;
		protected Action<T> Setter;
		public void Bind(IBindableVariable<T> other)
		{
			Getter = () => other.Value;
			Setter = (evt) => other.Value = evt;
		}
		public override void Bind(SharedVariable other)
		{
			if (other is IBindableVariable<T> variable)
			{
				Bind(variable);
			}
			else
			{
				Debug.LogError($"Variable named with {Name} bind failed!");
			}
		}
		public override void Unbind()
		{
			Getter = null;
			Setter = null;
		}
		[SerializeField]
		protected T value;
		public override ObserveVariable Observe()
		{
			return ObserveT();
		}
		public ObserveVariable<T> ObserveT()
		{
			var proxy = new ObserveVariable<T>(this);
			Setter ??= (evt) => { value = evt; };
			Setter += (evt) => proxy.OnValueChange.Invoke(evt);
			return proxy;
		}
	}
	/// <summary>
	/// Proxy variable to observe value change
	/// </summary>
	public class ObserveVariable
	{
		public Action<object> OnValueChange;
	}
	public class ObserveVariable<T> : ObserveVariable, IBindableVariable<T>
	{
		public T Value
		{
			get
			{
				return Getter();
			}
			set
			{
				Setter(value);
			}
		}
		private Func<T> Getter;
		private Action<T> Setter;
		public void Bind(IBindableVariable<T> other)
		{
			Getter = () => other.Value;
			Setter = (evt) => other.Value = evt;
		}
		public ObserveVariable(SharedVariable<T> variable)
		{
			Bind(variable);
		}
	}
}