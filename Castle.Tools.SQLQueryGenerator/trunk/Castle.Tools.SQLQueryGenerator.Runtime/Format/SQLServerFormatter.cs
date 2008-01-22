namespace Castle.Tools.SQLQueryGenerator.Runtime.Format
{
	public class SQLServerFormatter : IFormatter
	{
		public string Format(Model.Field.IFormatableField field)
		{
			string fieldName = field.Table + ".[" + field.Name + "]";
			if (field.Alias == null)
				return fieldName;

			return fieldName + "AS [" + field.Alias + "]";
		}

		public string Format(Model.Table.IFormatableTable table)
		{
			return "[" + table.Schema + "].[" + table.Name + "]";
		}
	}
}
