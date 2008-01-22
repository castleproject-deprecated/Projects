namespace Castle.Tools.SQLQueryGenerator.Runtime.Queries
{
	public class SQLSelectQuery : SQLQuery
	{
		readonly Clauses.SelectClause selectClause;
		Clauses.FromClause fromClause;
		Clauses.WhereClause whereClause;
		public SQLSelectQuery(Clauses.SelectClause selectClause)
		{
			this.selectClause = selectClause;
		}

		public override string ToString()
		{
			System.Text.StringBuilder select = new System.Text.StringBuilder()
				.AppendLine(selectClause);

			if (fromClause != null)
				select.AppendLine(fromClause);

			if (whereClause != null)
				select.AppendLine(whereClause);

			return select.ToString();
		}

		public SQLSelectQuery From(Model.Table table)
		{
			fromClause = new Clauses.FromClause(table);
			return this;
		}

		public SQLSelectQuery Join(Model.Table table, Expressions.WhereExpression on)
		{
			fromClause.Join(new Expressions.JoinExpression(table, on));
			return this;
		}

		public SQLSelectQuery Where(Expressions.WhereExpression where)
		{
			whereClause = new Clauses.WhereClause(where);
			return this;
		}

		public static implicit operator string(SQLSelectQuery q)
		{
			return q.ToString();
		}
	}
}
