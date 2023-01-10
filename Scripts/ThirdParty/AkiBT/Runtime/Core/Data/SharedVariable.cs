using UnityEngine;
using System;
namespace Kurisu.AkiBT
{
	[System.Serializable]
public abstract class SharedVariable:ICloneable
{
		public SharedVariable()
		{
			
		}
		/// <summary>
		/// 是否共享
		/// </summary>
		/// <value></value>
		public bool IsShared
		{
			get=>isShared;
			set=>isShared=value;
		}
		[SerializeField]
		private bool isShared;
		public string Name
		{
			get
			{
				return this.mName;
			}
			set
			{
				this.mName = value;
			}
		}
		public abstract object GetValue();
		public abstract void SetValue(object value);

        public abstract object Clone();

        [SerializeField]
		private string mName;
	}
	[System.Serializable]
    public abstract class SharedVariable<T> : SharedVariable
	{
		public T Value
		{
			get
			{
				return (this.Getter == null) ? this.value : this.Getter();
			}
			set
			{
				if (this.Setter != null)
				{
					this.Setter(value);
				}
				else
				{
					this.value = value;
				}
			}
		}
		public sealed override object GetValue()
		{
			return this.Value;
		}
		public sealed override void SetValue(object value)
		{
			if (this.Setter != null)
			{
				this.Setter((T)((object)value));
			}
			else if (value is IConvertible)
			{
				this.value = (T)((object)Convert.ChangeType(value, typeof(T)));
			}
			else
			{
				this.value = (T)((object)value);
			}
		}
		private Func<T> Getter;
		private Action<T> Setter;
		/// <summary>
		/// 绑定共享变量
		/// </summary>
		/// <param name="other"></param>
		public void Bind(SharedVariable<T> other)
		{
			Getter=()=>other.Value;
			Setter=(evt)=>other.Value=evt;
		}
		[SerializeField]
		protected T value;
	} 
}