namespace Castle.Tools.QuerySpecBuilder.Builder.Expressions
{
	public class ExistsExpression
	{
		readonly QueryBuilder queryBuilder;

		public ExistsExpression(QueryBuilder queryBuilder)
		{
			this.queryBuilder = queryBuilder;
		}
	}
}