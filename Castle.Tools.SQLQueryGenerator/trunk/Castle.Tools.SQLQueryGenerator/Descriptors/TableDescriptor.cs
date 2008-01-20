using System;

namespace Castle.Tools.SQLQueryGenerator.Descriptors
{
	using System.Collections.Generic;
	using DatabaseMetadataProviders;

	public class TableDescriptor
	{
		readonly IDictionary<string, DbPropertyMetadata> properties = new Dictionary<string, DbPropertyMetadata>();

		public TableDescriptor(string table) : this(null, table)
		{
		}

		public TableDescriptor(string schema, string table)
		{
			if (string.IsNullOrEmpty(schema))
				Name = table;
			else
				Name = schema + "." + table;
		}

		public readonly string Name;

		public void Add(DbPropertyMetadata property)
		{
			if (properties.ContainsKey(property.Column))
				throw new ArgumentException(string.Format(
					"Duplicate property found: {0}.{1}", Name, property.Column), 
					"property");

			properties.Add(property.Column, property);
		}

		public ICollection<DbPropertyMetadata> Properties
		{
			get { return properties.Values; }
		}
	}
}