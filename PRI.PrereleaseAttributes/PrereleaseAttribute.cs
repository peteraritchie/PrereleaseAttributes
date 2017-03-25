using System;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace PRI.PrereleaseAttributes
{
#if !(SILVERLIGHT || WINDOWS_PHONE || NETFX_CORE || PORTABLE)
	[Serializable]
#endif
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Delegate | AttributeTargets.Assembly, Inherited = false)]
	public class PrereleaseAttribute : Attribute
	{
		public string Message { get; }

		public PrereleaseAttribute()
		{
		}

		public PrereleaseAttribute(string message)
		{
			Message = message;
		}
	}
}