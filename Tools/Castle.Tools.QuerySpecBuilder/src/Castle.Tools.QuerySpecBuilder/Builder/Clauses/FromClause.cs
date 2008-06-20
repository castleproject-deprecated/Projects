#region license

// Copyright 2008 Ken Egozi http://www.kenegozi.com/
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

namespace Castle.Tools.QuerySpecBuilder.Builder.Clauses
{
    using System.Collections.Generic;
	using Expressions;
    using Helpers;


	public class FromClause : AbstractClause
	{
		readonly List<JoinExpression> joins = new List<JoinExpression>();
		Expression root;

		public FromClause(QueryBuilder owningQuery)
			:base(owningQuery)
		{
		}

		public FromClause Table(string table, string alias)
		{
			root = new Expression(table).As(alias);
			return this;
		}

		public FromClause Table(string table)
		{
			root = new Expression(table);
			return this;
		}

		public FromClause Query(QueryBuilder query, string alias)
		{
			owningQuery.SubQueries.Add(query);
			query.IsSubQuery = true;
			root = new Expression(query).Parethesise().As(alias);
			return this;
		}
		
		public FromClause Add(JoinExpression join)
		{
			joins.Add(join);
			return this;
		}

		public override string GetQueryString()
		{
			var buf = new IndentingStringBuilder();
			buf.AppendLine("FROM");
			if (joins.Count == 0)
			{
				buf
					.In(1)
					.AppendLine(root.GetQueryString())
					.Out(1);
			}
			else
			{
				buf
					.In(4)
					.AppendLine(root.GetQueryString())
					.Out(3);

				foreach (var join in joins)
				{
					buf.AppendLine(join.GetQueryString());
				}
				buf.Out(1);
			}

			return buf.ToString();
		}
	}
}