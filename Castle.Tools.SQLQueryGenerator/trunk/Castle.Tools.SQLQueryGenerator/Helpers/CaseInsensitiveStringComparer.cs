using System;
using System.Collections.Generic;

namespace Castle.Tools.SQLQueryGenerator.Helpers
{
	public class CaseInsensitiveStringComparer : IEqualityComparer<string>
	{
		#region IEqualityComparer<string> Members

		public bool Equals(string x, string y)
		{
			return x.Equals(y, StringComparison.InvariantCultureIgnoreCase);
			throw new System.Exception("The method or operation is not implemented.");
		}

		public int GetHashCode(string obj)
		{
			return obj.ToLowerInvariant().GetHashCode();
		}
		#endregion
	}
}