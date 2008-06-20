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
namespace Castle.Tools.SQLQueryGenerator
{
	using System.Collections.Generic;
	using DatabaseMetadataProviders;
	using Descriptors;
	using Helpers;

	public class MetadataProcessor
	{
		IDictionary<string, TableDescriptor> tables;

		public TableDescriptor GetTableDescriptorFrom(DbPropertyMetadata propertyMetadata, bool schemaInClassName)
		{
			TableDescriptor table = new TableDescriptor(propertyMetadata.Schema, propertyMetadata.Table, schemaInClassName);
			if (tables.ContainsKey(table.ClassName))
				return tables[table.ClassName];

			tables.Add(table.ClassName, table);
			return table;
		}

		public void Process(DbPropertyMetadata propertyMetadata, bool schemaInClassName)
		{
			TableDescriptor table = GetTableDescriptorFrom(propertyMetadata, schemaInClassName);
			table.Add(propertyMetadata);
		}

		public IDictionary<string, TableDescriptor> GetTableDescriptorsFrom(IEnumerable<DbPropertyMetadata> metadata, bool schemaInClassName)
		{
			tables = new Dictionary<string, TableDescriptor>(new CaseInsensitiveStringComparer());

			foreach (DbPropertyMetadata propertyMetadata in metadata)
			{
				Process(propertyMetadata, schemaInClassName);
			}
			return tables;
		}
	}
}
