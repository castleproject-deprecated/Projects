namespace Castle.Tools.SQLQueryGenerator.Runtime.Format
{
	public class SQLServerFormatter : IFormatter
	{
		public string Format(Model.Field.IFormatableField field)
		{
			return field.Table + ".[" + field.Name + "]";
		}

		public string FormatForSelectClause(Model.Field.IFormatableField field)
		{
			string fieldName = Format(field);
			if (field.Alias != null)
				fieldName += " AS [" + field.Alias + "]";

			return fieldName;
		}
		
		public string Format(Model.Table.IFormatableTable table)
		{
			if (table.Alias != null)
				return "[" + table.Alias + "]";

			return GetTableNameFrom(table);
		}

		static string GetTableNameFrom(Model.Table.IFormatableTable table)
		{
			return "[" + table.Schema + "].[" + table.Name + "]";
		}

		public string FormatForFromClause(Model.Table.IFormatableTable table)
		{
			string tableName = GetTableNameFrom(table);

			if (table.Alias != null)
				tableName += " AS [" + table.Alias + "]";

			return tableName;
		}
	}
}



/*

 * SELECT:		T.Alias + Field.Name + "AS" + Field.Alias
 * 
 * FROM			T.Name AS T.Alias
 * 
 *	JOIN ON    T.Alias + Field.Name
 *   WHERE     T.Alias + Field.Name
 * 
 * SELECT T1.Id as MyDamnId
 * FROM			dbo.Tbl1 AS T1
 *		JOIN	dbo.Tbl2 AS T2 ON T1.Id = T2.Id
 * WHERE		T1.Id = 12
 
 
 
 
 
 */