namespace Castle.Tools.SQLQueryGenerator.Runtime.Model.Field
{
	public interface IFormatableField
	{
		Table.AbstractTable Table { get; }
		string Name { get; }
		string Alias { get; }
	}
}