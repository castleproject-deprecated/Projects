namespace Castle.Tools.SQLQueryGenerator.Runtime.Clauses
{
	public class FromClause : AbstractSqlClause
	{
		readonly Model.Table.AbstractTable table;
		readonly System.Collections.Generic.List<Expressions.JoinExpression> joins = new System.Collections.Generic.List<Expressions.JoinExpression>();
		public FromClause(Model.Table.AbstractTable table)
		{
			this.table = table;
		}

		public FromClause Join(Expressions.JoinExpression join)
		{
			joins.Add(join);
			return this;
		}

		public FromClause Join(Model.Table.AbstractTable table, Expressions.WhereExpression on)
		{
			joins.Add(new Expressions.JoinExpression(table, on));
			return this;
		}

		public override string ToString()
		{
			System.Text.StringBuilder from = new System.Text.StringBuilder()
				.AppendLine("\t\t\t\t" + Format.Formatting.FormatForFromClause(table));

			foreach (Expressions.JoinExpression join in joins)
			{
				from.AppendLine(join.ToString());
			}

			from
				.Insert(0, "FROM" + System.Environment.NewLine);

			return from.ToString();
		}	
	}
}
