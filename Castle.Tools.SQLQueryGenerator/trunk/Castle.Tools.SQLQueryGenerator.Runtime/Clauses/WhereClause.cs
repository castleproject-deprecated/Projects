namespace Castle.Tools.SQLQueryGenerator.Runtime.Clauses
{
	public class WhereClause : AbstractSqlClause
	{
		readonly Expressions.WhereExpression whereExpression;

		public WhereClause(Expressions.WhereExpression whereExpression)
		{
			this.whereExpression = whereExpression;
		}

		public override string ToString()
		{
			System.Text.StringBuilder where = new System.Text.StringBuilder()
				.AppendLine("WHERE")
				.AppendLine(whereExpression.ToWhereString());

			return where.ToString();
		}	
	}
}
