namespace Castle.Tools.SQLQueryGenerator.Runtime.Clauses
{
	public class FromClause : AbstractSqlClause
	{
		readonly Model.Table.ITable table;
		readonly System.Collections.Generic.List<Expressions.JoinExpression> joins = new System.Collections.Generic.List<Expressions.JoinExpression>();
		public FromClause(Model.Table.ITable table)
		{
			this.table = table;
		}

		public FromClause Join(Expressions.JoinExpression join)
		{
			joins.Add(join);
			return this;
		}

		public FromClause Join(Model.Table.ITable table, Expressions.WhereExpression on)
		{
			joins.Add(new Expressions.JoinExpression(table, on));
			return this;
		}

		public override string ToString()
		{
			System.Text.StringBuilder from = new System.Text.StringBuilder()
				.Append(table);
			foreach (Expressions.JoinExpression join in joins)
			{
				from
					.Insert(0, '(')
					.Append(join)
					.Append(')');
			}

			from.Insert(0, "FROM ");

			return from + " ";
		}	
	}
}
