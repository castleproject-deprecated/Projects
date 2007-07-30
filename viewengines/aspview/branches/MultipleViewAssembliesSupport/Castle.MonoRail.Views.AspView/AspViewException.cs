using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Castle.MonoRail.Views.AspView
{
	public class AspViewException : Exception
	{
		public AspViewException(string message) : base(message) { }
		public AspViewException(Exception innerException, string message) : base(message, innerException) { }
		public AspViewException(string message, params object[] arguments) : this(string.Format(message, arguments)) { }
		public AspViewException(Exception innerException, string message, params object[] arguments) : this(innerException, string.Format(message, arguments)) { }
	}
}
