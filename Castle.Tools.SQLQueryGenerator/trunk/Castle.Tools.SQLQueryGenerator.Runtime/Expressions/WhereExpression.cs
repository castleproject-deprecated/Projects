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

		public virtual string ToOnString()
		{
			return "\t\t\t\t\t" + Expression;
		}

		public virtual string ToWhereString()
		{
			return "\t\t\t\t" + Expression;
		}
	}
}
