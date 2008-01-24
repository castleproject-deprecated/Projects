namespace Castle.Tools.SQLQueryGenerator.Runtime.Clauses
{
	public class SelectClause : AbstractSqlClause
	{
		readonly System.Collections.Generic.IEnumerable<Model.Field.IFormatableField> fields;

		public SelectClause(System.Collections.Generic.IEnumerable<Model.Field.IFormatableField> fields)
		{
			this.fields = fields;
		}

		System.Collections.Generic.IEnumerable<Model.Field.IFormatableField> Fields { get { return fields; } }
		public override string ToString()
		{
			System.Text.StringBuilder select = new System.Text.StringBuilder()
				.AppendLine("SELECT");

			System.Collections.Generic.List<string> fieldNames = new System.Collections.Generic.List<string>();

			foreach (Model.Field.IFormatableField field in Fields)
				fieldNames.Add("\t\t\t\t" + Format.Formatting.FormatForSelectClause(field));

			select
				.Append(string.Join("," + System.Environment.NewLine , fieldNames.ToArray()))
				.AppendLine();

			return select.ToString();
		}
	}
}
