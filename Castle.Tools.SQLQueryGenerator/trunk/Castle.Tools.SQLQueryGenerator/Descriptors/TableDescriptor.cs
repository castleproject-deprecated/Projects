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

namespace Castle.Tools.SQLQueryGenerator.Descriptors
{
	using System;
	using System.Collections.Generic;
	using Helpers;
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