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

namespace Castle.Tools.SQLQueryGenerator.Runtime.Queries
{
	public class SQLSelectQuery : SQLQuery
	{
		readonly Clauses.SelectClause selectClause;
		Clauses.FromClause fromClause;
		Clauses.WhereClause whereClause;
		public SQLSelectQuery(Clauses.SelectClause selectClause)
		{
			this.selectClause = selectClause;
		}

		public override string ToString()
		{
			System.Text.StringBuilder select = new System.Text.StringBuilder()
				.Append(selectClause);

			if (fromClause != null)
				select.Append(fromClause);

			if (whereClause != null)
				select.Append(whereClause);

			return select.ToString();
		}

		public SQLSelectQuery From(Model.Table.AbstractTable table)
		{
			return From(new Clauses.FromClause(table));
		}

		public SQLSelectQuery From(Clauses.FromClause from)
		{
			fromClause = from;
			return this;
		}
		
		public SQLSelectQuery Join(Model.Table.AbstractTable table, Expressions.WhereExpression on)
		{
			fromClause.Join(new Expressions.JoinExpression(table, on));
			return this;
		}

		public SQLSelectQuery Where(Expressions.WhereExpression where)
		{
			return Where(new Clauses.WhereClause(where));
		}

		public SQLSelectQuery Where(Clauses.WhereClause where)
		{
			whereClause = where;
			return this;
		}

		public static implicit operator string(SQLSelectQuery q)
		{
			return q.ToString();
		}
	}
}
