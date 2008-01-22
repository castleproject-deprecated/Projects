namespace Castle.Tools.SQLQueryGenerator.Runtime.Clauses
{
	public class SelectClause : AbstractSqlClause
	{
		readonly System.Collections.Generic.IEnumerable<Model.Field.IField> fields;

		public SelectClause(System.Collections.Generic.IEnumerable<Model.Field.IField> fields)
		{
			this.fields = fields;
		}

		System.Collections.Generic.IEnumerable<Model.Field.IField> Fields { get { return fields; } }
		public override string ToString()
		{
			System.Text.StringBuilder select = new System.Text.StringBuilder()
				.Append("SELECT ");
			System.Collections.Generic.List<string> fieldNames = new System.Collections.Generic.List<string>();

			foreach (Model.Field.IField field in Fields)
				fieldNames.Add(field.ToString());
			select.Append(string.Join(", ", fieldNames.ToArray()));

			select.Append(' ');
			return select.ToString();
		}
	}
}
