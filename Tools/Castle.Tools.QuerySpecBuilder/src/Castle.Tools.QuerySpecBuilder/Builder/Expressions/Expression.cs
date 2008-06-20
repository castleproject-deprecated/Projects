namespace Castle.Tools.QuerySpecBuilder.Builder.Expressions
{
	using Expressions;

	public class Expression : IExpression
	{
		readonly object expression;
		internal string Alias {get; private set;}
		bool parethesise;

		public Expression(string literal)
		{
			expression = literal;
		}

		public Expression(QueryBuilder query)
		{
			expression = query;
		}

		public Expression As(string alias)
		{
			Alias = alias;
			return this;
		}

		public Expression Parethesise()
		{
			parethesise = true;
			return this;
		}

		public override string ToString()
		{
			return GetQueryString();
		}

		string GetExpressionString()
		{
			if (expression is IExpression)
				return ((IExpression)expression).GetQueryString();

			if (expression is QueryBuilder)
				return ((QueryBuilder)expression).GetQueryString();

			return expression.ToString();
		}

		public virtual string GetQueryString()
		{
			var exp = GetExpressionString();
			
			if (parethesise)
				exp = "(" + exp + ")";

			if (Alias == null)
				return exp;

			return exp + " AS " + Alias;
		}
	}
}