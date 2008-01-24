namespace Castle.Tools.SQLQueryGenerator.Runtime.Format
{
	public static class Formatting
	{
		static IFormatter formatter = new SQLServerFormatter();

		public static void SetFormatterTo(IFormatter newFormatter)
		{
			formatter = newFormatter;
		}

		public static string Format(Model.Table.AbstractTable table)
		{
			return formatter.Format(table);
		}

		public static string FormatForFromClause(Model.Table.IFormatableTable table)
		{
			return formatter.FormatForFromClause(table);
		}

		public static string Format(Model.Field.AbstractField field)
		{
			return formatter.Format(field);
		}

		public static string FormatForSelectClause(Model.Field.IFormatableField field)
		{
			return formatter.FormatForSelectClause(field);
		}
	}
}