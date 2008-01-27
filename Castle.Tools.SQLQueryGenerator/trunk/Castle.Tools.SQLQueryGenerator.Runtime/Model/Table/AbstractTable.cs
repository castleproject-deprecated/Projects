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

namespace Castle.Tools.SQLQueryGenerator.Runtime.Model.Table
{
	public abstract class AbstractTable : IFormatableTable
	{
		public AbstractTable(string schema, string name, string alias)
		{
			this.schema = schema;
			this.name = name;
			this.alias = alias;
		}

		public AbstractTable(string schema, string name)
		{
			this.schema = schema;
			this.name = name;
		}

		readonly string schema;
		readonly string name;
		readonly string alias;

		string IFormatableTable.Schema { get { return schema; } }
		string IFormatableTable.Name { get { return name; } }
		string IFormatableTable.Alias { get { return alias; } }

		public override string ToString()
		{
			return Format.Formatting.Format(this);
		}

	}
}
