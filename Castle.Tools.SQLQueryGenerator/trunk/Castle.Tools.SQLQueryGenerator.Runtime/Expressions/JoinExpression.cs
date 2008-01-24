namespace Castle.Tools.SQLQueryGenerator.Runtime.Expressions
{
	public class JoinExpression
	{
		readonly Model.Table.AbstractTable table;
		readonly WhereExpression on;

		public JoinExpression(Model.Table.AbstractTable table, WhereExpression on)
		{
			this.table = table;
			this.on = on;
		}

		public override string ToString()
		{
			return "\tJOIN\t\t" + Format.Formatting.FormatForFromClause(table) + " ON" + System.Environment.NewLine + on.ToOnString();
		}

	}
}
