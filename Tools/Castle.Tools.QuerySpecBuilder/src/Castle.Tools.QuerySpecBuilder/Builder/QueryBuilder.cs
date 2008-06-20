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

namespace Castle.Tools.QuerySpecBuilder.Builder
{
	using System.Collections.Generic;
	using Expressions;
	using Clauses;
    using Helpers;

	public class QueryBuilder
	{
		SelectClause selectClause;
		FromClause fromClause;
		WhereClause whereClause;
		GroupByClause groupByClause;
		HavingClause havingClause;
		OrderByClause orderByClause;
		readonly IList<QueryBuilder> subqueries = new List<QueryBuilder>();
		internal bool IsSubQuery { get; set; }

		public SelectClause Select
		{
			get
			{
				if (selectClause == null)
					selectClause = new SelectClause(this);
				return selectClause;
			}
		}

		public FromClause From
		{
			get
			{
				if (fromClause == null)
					fromClause = new FromClause(this);
				return fromClause;
			}
		}

		public WhereClause Where
		{
			get
			{
				if (whereClause == null)
					whereClause = new WhereClause(this);
				return whereClause;
			}
		}

		public GroupByClause GroupBy
		{
			get
			{
				if (groupByClause == null)
					groupByClause = new GroupByClause(this);
				return groupByClause;
			}
		}

		public HavingClause Having
		{
			get
			{
				if (havingClause == null)
					havingClause = new HavingClause(this);
				return havingClause;
			}
		}

		public OrderByClause OrderBy
		{
			get
			{
				if (orderByClause == null)
					orderByClause = new OrderByClause(this);
				return orderByClause;
			}
		}

		bool isRendering;
		/// <summary>
		/// Gets the SQL string described by this builder.
		/// IMPORTANT: this method is NOT thread safe
		/// </summary>
		/// <returns>SQL query string</returns>
		public virtual string GetQueryString()
		{
			if (isRendering == false && Wrapper != null)
			{
				isRendering = true;
				return Wrapper.GetQueryString();
			}


			var buf = new IndentingStringBuilder();
			if (selectClause == null)
				selectClause = new SelectClause(this);
			if (selectClause != null)
				buf.AppendLine(selectClause.GetQueryString());

			if (fromClause != null)
				buf.AppendLine(fromClause.GetQueryString());

			if (whereClause != null)
				buf.AppendLine(whereClause.GetQueryString());

			if (groupByClause != null)
				buf.AppendLine(groupByClause.GetQueryString());

			if (havingClause != null)
				buf.AppendLine(havingClause.GetQueryString());

			if (orderByClause != null)
			{
				if (IsSubQuery == false)
					buf.AppendLine(orderByClause.GetQueryString());
			}

			isRendering = false;

			return buf.ToString();

		}

		const string SORT_FIELD = "QUERY_BUILDER_SORTBY_FIELD";
		const string ROWNUM_FIELD = "QUERY_BUILDER_ROWNUM_FIELD";
		const string INTERNAL_QUERY = "QUERY_BUILDER_INTERNAL_QUERY";
		const string MIDDLE_QUERY = "QUERY_BUILDER_MIDDLE_QUERY";
		bool hasPagingWrapper;
		internal QueryBuilder Wrapper;

		public QueryBuilder SetFirstResult(int firstResult)
		{
			if (hasPagingWrapper == false)
				BuildPagingWrapper();
			Wrapper.Wrapper.Where.Add(MIDDLE_QUERY + "." + ROWNUM_FIELD + " > " + firstResult);
			return this;
		}

		public QueryBuilder SetMaxResults(int maxResults)
		{
			if (hasPagingWrapper == false)
				BuildPagingWrapper();

			Wrapper.Wrapper.Select.Top(maxResults);
			return this;
		}

		void BuildPagingWrapper()
		{
			var orderBy = OrderBy.GetQueryString();
			var orderByExpression = orderBy.Substring(8);

			// create middleQuery wrapper
			var middleQuery = new QueryBuilder();
			middleQuery.Select.Add("ROW_NUMBER() OVER(ORDER BY " + SORT_FIELD + " )", ROWNUM_FIELD);

			foreach (var expression in Select.Expressions)
			{
				var exp = (Expression)expression;
				middleQuery.Select.Add(INTERNAL_QUERY + "." + exp.Alias);
			}
			middleQuery.Select.Add(INTERNAL_QUERY + "." + SORT_FIELD);
			middleQuery.From.Query(this, INTERNAL_QUERY);

			// create outer query

			var outerQuery = new QueryBuilder();

			foreach (var expression in Select.Expressions)
			{
				var exp = (Expression)expression;
				outerQuery.Select.Add(exp.Alias);
			}

			// Add sort field
			Select.Add(orderByExpression, SORT_FIELD);

			outerQuery.From.Query(middleQuery, MIDDLE_QUERY);

			outerQuery.OrderBy.Add(SORT_FIELD);

			Wrapper = middleQuery;
			middleQuery.Wrapper = outerQuery;
			hasPagingWrapper = true;

			IsSubQuery = true;
			Wrapper.IsSubQuery = true;
		}

		public IList<QueryBuilder> SubQueries { get { return subqueries; } }

	}
}