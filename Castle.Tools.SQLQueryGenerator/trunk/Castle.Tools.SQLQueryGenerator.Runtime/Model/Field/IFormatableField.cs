namespace Castle.Tools.SQLQueryGenerator.Runtime.Model.Field
{
	public interface IFormatableField : IAliasedField, INonAliasedField
	{
		Table.AbstractTable Table { get; }
		string Name { get; }
		string Alias { get; }
	}
}