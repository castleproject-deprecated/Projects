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
	using System.Collections.Generic;
	using Expressions;

	public class OrderByClause : AbstractSqlClause
	{
		public readonly OrderByExpression[] orders;

		public OrderByClause(params OrderByExpression[] orders)
		{
			this.orders = orders;
		}

		public override string ToString()
		{
			System.Text.StringBuilder where = new System.Text.StringBuilder()
				.AppendLine("ORDER BY");

			where.Append(string.Join(@",
", 
				OrdersToStrings(orders)));

			where.AppendLine();

			return where.ToString();
		}

		private static string[] OrdersToStrings(OrderByExpression[] orders)
		{
			List<string> strings = new List<string>(orders.Length);
			foreach (OrderByExpression order in orders)
			{
				strings.Add(order.ToString());
			}
			return strings.ToArray();
		}
	}
}
