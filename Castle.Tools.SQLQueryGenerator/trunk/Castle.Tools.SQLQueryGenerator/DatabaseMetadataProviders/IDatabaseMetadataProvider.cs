namespace Castle.Tools.SQLQueryGenerator.DatabaseMetadataProviders
{
	using System.Collections.Generic;

	public interface IDatabaseMetadataProvider
	{
		IEnumerable<DbPropertyMetadata> ExtractMetadata();
	}
}