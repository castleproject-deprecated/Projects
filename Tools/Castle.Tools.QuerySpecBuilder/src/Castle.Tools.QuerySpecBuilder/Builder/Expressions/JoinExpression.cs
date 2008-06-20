namespace Castle.Tools.QuerySpecBuilder.Builder.Expressions
{
	using Expressions;

	public class JoinExpression : IExpression
	{
		JoinType joinType = JoinType.Inner;
		IExpression on;
		IExpression table;
		IExpression alias;

		public JoinExpression Left()
		{
			joinType = JoinType.Left;
			return this;
		}

		public JoinExpression Right()
		{
			joinType = JoinType.Right;
			return this;
		}

		public JoinExpression Table(string expression)
		{
			table = new Expression(expression);
			return this;
		}

		public JoinExpression As(string expression)
		{
			alias = new Expression(expression);
			return this;
		}

		public JoinExpression On(string expression)
		{
			on = new Expression(expression);
			return this;
		}

		public override string ToString()
		{
			return GetQueryString();
		}

		public virtual string GetQueryString()
		{
			var aliasedTable = alias == null
			                   	? table.GetQueryString()
			                   	: table + " AS " + alias;
			return string.Format("{0}{1} ON ({2})", joinType, aliasedTable, on);
		}
	}
}