namespace Castle.Tools.SQLQueryGenerator.Runtime
{
	public class SQLQuery
	{
		public static Queries.SQLSelectQuery Select(params Model.Field[] fields)
		{
			Clauses.SelectClause selectClause = new Clauses.SelectClause(fields);
			Queries.SQLSelectQuery q = new Queries.SQLSelectQuery(selectClause);
			return q;
		}

		public static implicit operator string(SQLQuery q)
		{
			return q.ToString();
		}
	}
}
