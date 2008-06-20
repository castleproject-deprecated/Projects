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
	using System.Linq;
	using System.Collections.Generic;
	using Expressions;
	using Helpers;

	public abstract class AbstractClause
	{
		protected readonly QueryBuilder owningQuery;
		readonly List<IExpression> expressions = new List<IExpression>();

		protected virtual List<IExpression> Expressions
		{
			get { return expressions; }
		}

		protected AbstractClause(QueryBuilder owningQuery)
		{
			this.owningQuery = owningQuery;
		}

		public QueryBuilder End { get { return owningQuery; } }

		public virtual string GetQueryString()
		{
			var buf = new IndentingStringBuilder();
			buf.AppendLine(ClauseVerb);
			buf.In(1);
			buf.Append(GetExpressionsString());
			buf.Out(1);
			return buf.ToString();
		}

		protected virtual string ClauseVerb { get { throw new NotImplementedException(); } }

		protected virtual string ExpressionsSeperator
		{
			get { return "," + Environment.NewLine; }
		}

		protected virtual string GetExpressionsString()
		{
			return string.Join(ExpressionsSeperator, Expressions.Select(e => e.GetQueryString()).ToArray());
		}
	}
}