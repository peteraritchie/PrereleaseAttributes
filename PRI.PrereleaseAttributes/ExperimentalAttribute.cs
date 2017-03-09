﻿using System;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace PRI.PrereleaseAttributes
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Delegate | AttributeTargets.Assembly, Inherited = false)]
	public class ExperimentalAttribute : Attribute
	{
		public string Message { get; }

		public ExperimentalAttribute()
		{
		}

		public ExperimentalAttribute(string message)
		{
			Message = message;
		}
	}
}
