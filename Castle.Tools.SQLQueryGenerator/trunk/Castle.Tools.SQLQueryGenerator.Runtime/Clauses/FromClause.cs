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

namespace Castle.Tools.SQLQueryGenerator.Runtime.Clauses
{
	public class FromClause : AbstractSqlClause
	{
		readonly Model.Table.AbstractTable table;
		readonly System.Collections.Generic.List<Expressions.JoinExpression> joins = new System.Collections.Generic.List<Expressions.JoinExpression>();
		public FromClause(Model.Table.AbstractTable table)
		{
			this.table = table;
		}

		public FromClause Join(Expressions.JoinExpression join)
		{
			joins.Add(join);
			return this;
		}

		public FromClause Join(Model.Table.AbstractTable table, Expressions.WhereExpression on)
		{
			joins.Add(new Expressions.JoinExpression(table, on));
			return this;
		}

		public override string ToString()
		{
			System.Text.StringBuilder from = new System.Text.StringBuilder()
				.AppendLine("\t\t\t\t" + Format.Formatting.FormatForFromClause(table));

			foreach (Expressions.JoinExpression join in joins)
			{
				from.AppendLine(join.ToString());
			}

			from
				.Insert(0, "FROM" + System.Environment.NewLine);

			return from.ToString();
		}	
	}
}
