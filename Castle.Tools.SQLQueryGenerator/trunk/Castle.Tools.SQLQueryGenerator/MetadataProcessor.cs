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
