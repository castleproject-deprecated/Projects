#region license
// Copyright 2008 Ken Egozi http://www.kenegozi.com/blog
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

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
				"dbo", "MyTable", "Id", "int", false);

			TableDescriptor table = processor.GetTableDescriptorFrom(propertyMetadata, true);

			Assert.Equal(table.Name, "MyTable");
			Assert.Equal(table.Schema, "dbo");
		}

		[Fact]
		public void GetTableDescriptorFrom_WhenCalledWithExistingTableData_ReturnsTheExistingTable()
		{
			DbPropertyMetadata property1 = new DbPropertyMetadata(
				"dbo", "MyTable", "Id", "int", false);

			DbPropertyMetadata property2 = new DbPropertyMetadata(
				"dbo", "MyTable", "Name", "string", false);

			TableDescriptor table1 = processor.GetTableDescriptorFrom(property1, true);
			TableDescriptor table2 = processor.GetTableDescriptorFrom(property2, true);

			Assert.Equal(table1, table2);
		}

		[Fact]
		public void Process_WhenFindsNewTable_AddsTheTable()
		{
			GetTables().Add("Initial", new TableDescriptor("Initial", true));
			Assert.Contains("Initial", GetTables().Keys);

			processor.Process(new DbPropertyMetadata(
				"dbo", "MyTable", "Id", "int", false), true);

			Assert.Equal(2, GetTables().Keys.Count);

			Assert.Contains("dbo_MyTable", GetTables().Keys);
		}

		[Fact]
		public void Process_WhenFindsExistingTable_WillNotAddTheTable()
		{
			GetTables().Add("Initial", new TableDescriptor("Initial", true));
			Assert.Contains("Initial", GetTables().Keys);

			processor.Process(new DbPropertyMetadata(
				null, "Initial", "Id", "int", false), true);

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
