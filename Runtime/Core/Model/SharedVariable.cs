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
		/// Whether to use shared variable
		/// </summary>
		/// <value></value>
		public bool IsShared
		{
			get => isShared;
			set => isShared = value;
		}
		[SerializeField]
		private bool isShared;
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

		public abstract object Clone();

		[SerializeField]
		private string mName;
	}
	public interface IBindableVariable<K> where K : SharedVariable
	{
		/// <summary>
		/// Bind to other sharedVariable
		/// </summary>
		/// <param name="other"></param>
		public void Bind(K other);
	}
	[Serializable]
	public abstract class SharedVariable<T> : SharedVariable, IBindableVariable<SharedVariable<T>>
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
		public void Bind(SharedVariable<T> other)
		{
			Getter = () => other.Value;
			Setter = (evt) => other.Value = evt;
		}
		[SerializeField]
		protected T value;
	}
}