namespace Castle.Tools.SQLQueryGenerator.Runtime.Model.Table
{
	public interface IFormatableTable
	{
		string Schema { get; }
		string Name { get; }
		string Alias { get; }
	}
}