using System;
using Castle.Tools.SQLQueryGenerator.Helpers;

namespace Castle.Tools.SQLQueryGenerator.Descriptors
{
	using System.Collections.Generic;
	using DatabaseMetadataProviders;

	public class TableDescriptor
	{
		readonly IDictionary<string, DbPropertyMetadata> properties = new Dictionary<string, DbPropertyMetadata>();

		public readonly string Name;
		public readonly string Schema;
		public readonly string ClassName;

		public TableDescriptor(string table, bool schemaInClassName) : this(null, table, schemaInClassName)
		{
		}

		public TableDescriptor(string schema, string table, bool schemaInClassName)
		{
			Name = table;
			Schema = schema;
			ClassName = GetClassNameFrom(Schema, Name, schemaInClassName);
		}


		static string GetClassNameFrom(string schema, string name, bool schemaInClassName)
		{
			if (schemaInClassName == false)
				return Formatter.FormatClassNameFrom(name);

			string className = schema == null
				? name
				: schema + "." + name;

			return Formatter.FormatClassNameFrom(className);
		}

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