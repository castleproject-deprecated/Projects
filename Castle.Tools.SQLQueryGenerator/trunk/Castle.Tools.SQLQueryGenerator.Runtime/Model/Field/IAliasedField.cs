namespace Castle.Tools.SQLQueryGenerator.Runtime.Model.Field
{
	public interface IAliasedField : IField
	{
		INonAliasedField As(string alias);
	}
}