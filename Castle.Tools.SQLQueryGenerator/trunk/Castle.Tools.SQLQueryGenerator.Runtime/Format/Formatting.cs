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

		public static string Format(Model.Field.AbstractField field)
		{
			return formatter.Format(field);
		}
	}
}