using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Kurisu.AkiBT
{
	[System.Serializable]
public abstract class SharedVariable
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

		[SerializeField]
		private string mName;


	}
	[System.Serializable]
    public abstract class SharedVariable<T> : SharedVariable
	{
		
		public T Value
		{
			get=>this.value;
			
			set=>this.value = value;
		}

		public override object GetValue()
		{
			return this.Value;
		}

		public override void SetValue(object value)
		{
			
			if (value is IConvertible)
			{
				this.value = (T)((object)Convert.ChangeType(value, typeof(T)));
			}
			else
			{
				this.value = (T)((object)value);
			}
		}


		[SerializeField]
		public T value;
	}
	
	
    
}