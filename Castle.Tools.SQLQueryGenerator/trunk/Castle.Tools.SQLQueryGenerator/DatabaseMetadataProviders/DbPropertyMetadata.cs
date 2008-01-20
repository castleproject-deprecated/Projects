namespace Castle.Tools.SQLQueryGenerator.DatabaseMetadataProviders
{
	public class DbPropertyMetadata
	{
		public DbPropertyMetadata(string schema, string table, string column, string type, bool isNullable)
		{
			Schema = schema;
			Table=table;
			Column=column;
			Type = System.Type.GetType(type);
			IsNullable = isNullable;

			if (Type == null)
				System.Console.WriteLine(schema + "." + table + "." + type);
		}

		public readonly string Schema;
		public readonly string Table;
		public readonly string Column;
		public readonly System.Type Type;
		public readonly bool IsNullable;

		public override string ToString()
		{
			return string.Format("{0}.{1}.{2}.{3}.{4}",
				Schema, Table, Column, Type.Name, IsNullable ? "Nullable" : "Not Nullable");
		}

		public override bool Equals(object obj)
		{
			if (obj == null) return false;
			return ToString().Equals(obj.ToString());
		}

		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}
	}
}
