namespace Castle.Tools.SQLQueryGenerator.Tests.Mocks
{
	using System.Collections.Generic;
	using DatabaseMetadataProviders;

	public class MockDatabaseMetadataProvider : IDatabaseMetadataProvider
	{
		public IEnumerable<DbPropertyMetadata> ExtractMetadata()
		{
			return new DbPropertyMetadata[]
			{
				new DbPropertyMetadata("schema","Blog", "Id", "int", false), 
				new DbPropertyMetadata("schema","Blog", "Name", "string", false), 
				new DbPropertyMetadata("schema","Post", "Id", "int", false), 
				new DbPropertyMetadata("schema","Post", "BlogId", "int", false), 
				new DbPropertyMetadata("schema","Post", "Title", "string", false), 
				new DbPropertyMetadata("schema","Post", "Description", "string", true)
			};
		}
	}
}