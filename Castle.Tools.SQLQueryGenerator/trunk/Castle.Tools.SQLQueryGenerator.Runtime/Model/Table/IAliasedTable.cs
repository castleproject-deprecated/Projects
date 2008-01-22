namespace Castle.Tools.SQLQueryGenerator.Runtime.Model.Table
{
	public interface IAliasedTable : ITable
	{
		INonAliasedTable As(string alias);
	}
}