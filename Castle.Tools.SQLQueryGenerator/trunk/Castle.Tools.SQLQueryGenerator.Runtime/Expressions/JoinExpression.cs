namespace Castle.Tools.SQLQueryGenerator.Runtime.Expressions
{
	public class JoinExpression
	{
		readonly Model.Table table;
		readonly WhereExpression on;

		public JoinExpression(Model.Table table, WhereExpression on)
		{
			this.table = table;
			this.on = on;
		}

		public override string ToString()
		{
			return " JOIN " + table + " ON " + on;
		}

	}
}
