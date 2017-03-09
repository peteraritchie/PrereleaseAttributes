using System;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace PRI.PrereleaseAttributes
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Delegate | AttributeTargets.Assembly, Inherited = false)]
	public class PreviewAttribute : Attribute
	{
		public string Message { get; }

		public PreviewAttribute()
		{
		}

		public PreviewAttribute(string message)
		{
			Message = message;
		}
	}
}