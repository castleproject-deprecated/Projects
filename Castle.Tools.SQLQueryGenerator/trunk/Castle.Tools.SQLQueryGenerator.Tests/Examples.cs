using Castle.Tools.SQLQueryGenerator.Runtime;
using Castle.Tools.SQLQueryGenerator.Runtime.Model;
using Castle.Tools.SQLQueryGenerator.Tests.GeneratedClasses;
using Xunit;

namespace Castle.Tools.SQLQueryGenerator.Tests
{
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

		static void Spit(string sql)
		{
			System.Console.WriteLine(sql
				.Replace("\t", "[**]")
				.Replace("" + System.Environment.NewLine, "~" + System.Environment.NewLine)
				.Replace(" ", "%"));
		}
	}
}

