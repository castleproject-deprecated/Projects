namespace Castle.Tools.SQLQueryGenerator.Runtime.Model.Field
{
	public interface IAliasedField : IField
	{
		IField As(string alias);
	}
}