
namespace Castle.Tools.SQLQueryGenerator.Tests
{
	using System.Collections.Generic;
	using System.Reflection;
	using DatabaseMetadataProviders;
	using Descriptors;
	using Xunit;

	public class MetadataProcessorTests 
	{
		readonly MetadataProcessor processor;

		public MetadataProcessorTests()
		{
			processor = new MetadataProcessor();
			CreateTables();
		}

		[Fact]
		public void GetTableDescriptorFrom_Always_CreatesTheTableDescriptor()
		{
			DbPropertyMetadata propertyMetadata = new DbPropertyMetadata(
				"[dbo]", "[MyTable]", "[Id]", "int", false);

			TableDescriptor table = processor.GetTableDescriptorFrom(propertyMetadata);

			Assert.Equal(table.Name, "[dbo].[MyTable]");
		}

		[Fact]
		public void GetTableDescriptorFrom_WhenCalledWithExistingTableData_ReturnsTheExistingTable()
		{
			DbPropertyMetadata property1 = new DbPropertyMetadata(
				"[dbo]", "[MyTable]", "[Id]", "int", false);

			DbPropertyMetadata property2 = new DbPropertyMetadata(
				"[dbo]", "[MyTable]", "[Name]", "string", false);

			TableDescriptor table1 = processor.GetTableDescriptorFrom(property1);
			TableDescriptor table2 = processor.GetTableDescriptorFrom(property2);

			Assert.Equal(table1, table2);
		}

		[Fact]
		public void Process_WhenFindsNewTable_AddsTheTable()
		{
			GetTables().Add("[Initial]", new TableDescriptor("[Initial]"));
			Assert.Contains("[Initial]", GetTables().Keys);

			processor.Process(new DbPropertyMetadata(
				"[dbo]", "[MyTable]", "[Id]", "int", false));

			Assert.Equal(2, GetTables().Keys.Count);

			Assert.Contains("[dbo].[MyTable]", GetTables().Keys);
		}

		[Fact]
		public void Process_WhenFindsExistingTable_WillNotAddTheTable()
		{
			GetTables().Add("[Initial]", new TableDescriptor("[Initial]"));
			Assert.Contains("[Initial]", GetTables().Keys);

			processor.Process(new DbPropertyMetadata(
				null, "[Initial]", "[Id]", "int", false));

			Assert.Equal(1, GetTables().Keys.Count);
		}

		#region helpers
		private FieldInfo GetTablesField()
		{
			return processor.GetType().GetField("tables", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField);
		}

		private void CreateTables()
		{
			GetTablesField().SetValue(processor, new Dictionary<string, TableDescriptor>());
		}

		private IDictionary<string, TableDescriptor> GetTables()
		{
			return GetTablesField().GetValue(processor) as IDictionary<string, TableDescriptor>;
		}
		#endregion

	}
}
