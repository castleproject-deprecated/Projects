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

using Castle.Tools.SQLQueryGenerator.Runtime.Clauses;
using Castle.Tools.SQLQueryGenerator.Runtime.Expressions;

namespace Castle.Tools.SQLQueryGenerator.Tests
{
	using Runtime;
	using Runtime.Model;
	using GeneratedClasses;
	using Xunit;

	public class Examples
	{
		[Fact]
		public void SimpleSelect()
		{
			#region expected
			string expected =
@"SELECT
				[dbo].[Blogs].[Id],
				[dbo].[Blogs].[Name]
FROM
				[dbo].[Blogs]
";
			#endregion

			SQLQuery q = SQLQuery
				.Select(SQL.Blogs.Id, SQL.Blogs.Name)
				.From(SQL.Blogs);

			Assert.Equal(expected, q.ToString());
		}

		[Fact]
		public void SimpleSelectWithSimpleFieldAlias()
		{
			#region expected
			string expected =
@"SELECT
				[dbo].[Blogs].[Id] AS [MyId],
				[dbo].[Blogs].[Name]
FROM
				[dbo].[Blogs]
";
			#endregion

			SQLQuery q = SQLQuery
				.Select(SQL.Blogs.Id.As("MyId"), SQL.Blogs.Name)
				.From(SQL.Blogs);

			Assert.Equal(expected, q.ToString());
		}

		[Fact]
		public void SimpleSelectWithTableAlias()
		{
			#region expected
			string expected =
@"SELECT
				[MyBlogs].[Id] AS [MyId],
				[MyBlogs].[Name]
FROM
				[dbo].[Blogs] AS [MyBlogs]
";
			#endregion

			Tables_Blogs MyBlogs = SQL.Blogs.As("MyBlogs");

			SQLQuery q = SQLQuery
				.Select(MyBlogs.Id.As("MyId"), MyBlogs.Name)
				.From(MyBlogs);

			Assert.Equal(expected, q.ToString());
		}

		[Fact]
		public void SimpleSelectWithJoin()
		{
			#region expected
			string expected =
@"SELECT
				[dbo].[Blogs].[Id] AS [MyId],
				[dbo].[Blogs].[Name]
FROM
				[dbo].[Blogs]
	JOIN		[dbo].[Posts] ON
					([dbo].[Posts].[BlogId] = [dbo].[Blogs].[Id])
";
			#endregion

			SQLQuery q = SQLQuery
				.Select(SQL.Blogs.Id.As("MyId"), SQL.Blogs.Name)
				.From(SQL.Blogs)
					.Join(SQL.Posts, SQL.Posts.BlogId == SQL.Blogs.Id);
			
			Spit(expected);
			Spit(q);

			Assert.Equal(expected, q.ToString());
		}

		[Fact]
		public void SimpleSelectWithJoinAndTableAlias()
		{
			#region expected
			string expected =
@"SELECT
				[Message].[Id],
				[Message].[ParentId],
				[Message].[Content]
FROM
				[dbo].[ForumMessages] AS [Message]
	JOIN		[dbo].[ForumMessages] AS [Parent] ON
					([Message].[ParentId] = [Parent].[Id])
";
			#endregion

			Tables_ForumMessages Message = SQL.ForumMessages.As("Message");
			Tables_ForumMessages Parent = SQL.ForumMessages.As("Parent");

			SQLQuery q = SQLQuery
				.Select(Message.Id, Message.ParentId, Message.Content)
				.From(Message)
					.Join(Parent, Message.ParentId == Parent.Id);

			Assert.Equal(expected, q.ToString());
		}

		[Fact]
		public void SimpleSelectWithWhere()
		{
			#region expected
			string expected =
@"SELECT
				[dbo].[Blogs].[Id],
				[dbo].[Blogs].[Name]
FROM
				[dbo].[Blogs]
WHERE
				([dbo].[Blogs].[Id] = 20)
";
			#endregion

			SQLQuery q = SQLQuery
				.Select(SQL.Blogs.Id, SQL.Blogs.Name)
				.From(SQL.Blogs)
				.Where(SQL.Blogs.Id == 20);

			Assert.Equal(expected, q.ToString());
		}

		[Fact]
		public void SimpleSelectWithWhereAndParameter()
		{
			#region expected
			string expected =
@"SELECT
				[dbo].[Blogs].[Id],
				[dbo].[Blogs].[Name]
FROM
				[dbo].[Blogs]
WHERE
				([dbo].[Blogs].[Id] = @BlogId)
";
			#endregion

			Parameter<int> blogId = new Parameter<int>("BlogId");

			SQLQuery q = SQLQuery
				.Select(SQL.Blogs.Id, SQL.Blogs.Name)
				.From(SQL.Blogs)
				.Where(SQL.Blogs.Id == blogId);

			Assert.Equal(expected, q.ToString());
		}

		[Fact]
		public void ReusingClauses()
		{
			#region expected
			string expectedQ1 =
@"SELECT
				[dbo].[Blogs].[Id]
FROM
				[dbo].[Blogs]
WHERE
				([dbo].[Blogs].[Id] = 2)
";
			string expectedQ2 =
@"SELECT
				[dbo].[Blogs].[Name]
FROM
				[dbo].[Blogs]
WHERE
				([dbo].[Blogs].[Id] = 2)
";
			#endregion

			FromClause from = new FromClause(SQL.Blogs);
			WhereClause where = new WhereClause(SQL.Blogs.Id == 2);

			SQLQuery q1 = SQLQuery
				.Select(SQL.Blogs.Id)
				.From(from)
				.Where(where);

			SQLQuery q2 = SQLQuery
				.Select(SQL.Blogs.Name)
				.From(from)
				.Where(where);

			Assert.Equal(expectedQ1, q1.ToString());
			Assert.Equal(expectedQ2, q2.ToString());
		}
		
		[Fact]
		public void ComplexWhereClause()
		{
			#region expected
			string expectedQ1 =
@"SELECT
				[dbo].[Blogs].[Id]
FROM
				[dbo].[Blogs]
WHERE
				(([dbo].[Blogs].[Id] > 2) OR ([dbo].[Blogs].[Name] = N'Ken'))
";

			#endregion

			FromClause from = new FromClause(SQL.Blogs);
			WhereClause whereIdGraterThan2 = new WhereClause(SQL.Blogs.Id > 2);
			WhereClause whereIdNameIsKen = new WhereClause(SQL.Blogs.Name == "Ken");

			SQLQuery q1 = SQLQuery
				.Select(SQL.Blogs.Id)
				.From(from)
				.Where(whereIdGraterThan2 || whereIdNameIsKen);

			Assert.Equal(expectedQ1, q1.ToString());
		}

		[Fact]
		public void OrderBy()
		{
			#region expected
			string expected =
@"SELECT
				[dbo].[Blogs].[Id]
FROM
				[dbo].[Blogs]
WHERE
				([dbo].[Blogs].[Id] > 2)
ORDER BY
				[dbo].[Blogs].[Id],
				[dbo].[Blogs].[Name] DESC
";

			#endregion

			SQLQuery q = SQLQuery
				.Select(SQL.Blogs.Id)
				.From(SQL.Blogs)
				.Where(SQL.Blogs.Id > 2)
				.OrderBy(Order.By(SQL.Blogs.Id), Order.By(SQL.Blogs.Name).Desc);

			Assert.Equal(expected, q.ToString());
		}

		static void Spit(string sql)
		{
			System.Console.WriteLine(sql
				.Replace("\t", "[**]")
				.Replace("" + System.Environment.NewLine, "~" + System.Environment.NewLine)
				.Replace(" ", "%"));
		}
	}
}

