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

namespace Castle.Tools.SQLQueryGenerator.DatabaseMetadataProviders
{
	public class DbPropertyMetadata
	{
		public DbPropertyMetadata(string schema, string table, string column, string type, bool isNullable)
		{
			Schema = schema;
			Table = table;
			Column = column;
			Type = System.Type.GetType(type);
			IsNullable = isNullable;

			if (Type == null)
				System.Console.WriteLine(schema + "." + table + "." + type);
		}

		public readonly string Schema;
		public readonly string Table;
		public readonly string Column;
		public readonly System.Type Type;
		public readonly bool IsNullable;

		public override string ToString()
		{
			return string.Format("{0}.{1}.{2}.{3}.{4}",
				Schema, Table, Column, Type.Name, IsNullable ? "Nullable" : "Not Nullable");
		}

		public override bool Equals(object obj)
		{
			if (obj == null) return false;
			return ToString().Equals(obj.ToString());
		}

		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}
	}
}
