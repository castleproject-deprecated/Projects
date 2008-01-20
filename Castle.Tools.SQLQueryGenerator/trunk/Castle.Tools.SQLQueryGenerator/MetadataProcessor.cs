namespace Castle.Tools.SQLQueryGenerator
{
	using System.Collections.Generic;
	using DatabaseMetadataProviders;
	using Descriptors;
	using Helpers;

	public class MetadataProcessor
	{
		IDictionary<string, TableDescriptor> tables;

		public TableDescriptor GetTableDescriptorFrom(DbPropertyMetadata propertyMetadata)
		{
			TableDescriptor table = new TableDescriptor(propertyMetadata.Schema, propertyMetadata.Table);
			if (tables.ContainsKey(table.Name))
				return tables[table.Name];

			tables.Add(table.Name, table);
			return table;
		}

		public void Process(DbPropertyMetadata propertyMetadata)
		{
			TableDescriptor table = GetTableDescriptorFrom(propertyMetadata);
			table.Add(propertyMetadata);
		}

		public IDictionary<string, TableDescriptor> GetTableDescriptorsFrom(IEnumerable<DbPropertyMetadata> metadata)
		{
			tables = new Dictionary<string, TableDescriptor>(new CaseInsensitiveStringComparer());

			foreach (DbPropertyMetadata propertyMetadata in metadata)
			{
				Process(propertyMetadata);
			}
			return tables;
		}
	}
}
