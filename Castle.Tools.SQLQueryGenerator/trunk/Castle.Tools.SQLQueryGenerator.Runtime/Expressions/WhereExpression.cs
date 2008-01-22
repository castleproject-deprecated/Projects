namespace Castle.Tools.SQLQueryGenerator.Runtime.Expressions
{
	public class WhereExpression
	{
		public WhereExpression(Model.Field.AbstractField field, string operatorSign, object other)
		{
			string otherExpression = other.ToString();
			if (other is string)
				otherExpression = "'" + otherExpression.Replace("'", "''") + "'";
			Expression = "(" + field + operatorSign + otherExpression + ")";
		}

		public readonly string Expression;

		public override string ToString()
		{
			return Expression;
		}
	}
}
