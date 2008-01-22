namespace Castle.Tools.SQLQueryGenerator.Runtime.Clauses
{
	public class FromClause : AbstractSqlClause
	{
		readonly Model.Table table;
		readonly System.Collections.Generic.List<Expressions.JoinExpression> joins = new System.Collections.Generic.List<Expressions.JoinExpression>();
		public FromClause(Model.Table table)
		{
			this.table = table;
		}

		public void Join(Expressions.JoinExpression join)
		{
			joins.Add(join);
		}

		public override string ToString()
		{
			System.Text.StringBuilder select = new System.Text.StringBuilder()
				.Append(table);
			foreach (Expressions.JoinExpression join in joins)
			{
				select
					.Insert(0, '(')
					.Append(join)
					.Append(')');
			}

			select.Insert(0, "FROM ");

			return select.ToString();
		}	
	}
}
