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
	using System;
	using Expressions;

	public class WhereClause : AbstractClause
	{
		public WhereClause(QueryBuilder owningQuery)
			: base(owningQuery)
		{
		}

		public WhereClause Add(string expression)
		{
			Expressions.Add(new Expression(expression));
			return this;
		}

		protected override string ExpressionsSeperator
		{
			get
			{
				return Environment.NewLine + "and ";
			}
		}

		protected override string ClauseVerb
		{
			get
			{
				return "WHERE";
			}
		}
		protected override string GetExpressionsString()
		{
			return "\t" + base.GetExpressionsString();
		}
		public override string GetQueryString()
		{
			if (Expressions.Count == 0)
				return string.Empty;

			return base.GetQueryString();
		}
	}
}