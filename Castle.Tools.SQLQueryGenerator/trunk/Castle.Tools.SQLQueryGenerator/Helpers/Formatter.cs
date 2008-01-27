using System.Text.RegularExpressions;

namespace Castle.Tools.SQLQueryGenerator.Helpers
{
	public static class Formatter
	{
		static readonly Regex invalidChars = new Regex("[^\\w\\d_]");
		public static string FormatClassNameFrom(string name)
		{
			return invalidChars.Replace(name, "_");
		}
	}
}