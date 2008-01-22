using Castle.Tools.SQLQueryGenerator.Runtime.Clauses;
using Castle.Tools.SQLQueryGenerator.Runtime.Model.Table;
using Castle.Tools.SQLQueryGenerator.Tests.GeneratedClasses;
using Xunit;

namespace Castle.Tools.SQLQueryGenerator.Tests.RuntimeFixtures
{
	public class FromClauseTests
	{
		[Fact]
		public void ToString_Always_GeneratesCorrectSQLClause()
		{
			ITable table = new Tables_Blogs();

			FromClause from = new FromClause(table);

			string expected = "FROM [dbo].[Blogs] ";

			Assert.Equal(expected, from.ToString());
		}

		[Fact]
		public void ToString_WithAlias_GeneratesCorrectSQLClause()
		{
			ITable table = new Tables_Blogs().As("MyTable");

			FromClause from = new FromClause(table);

			string expected = "FROM [dbo].[Blogs] AS [MyTable] ";

			Assert.Equal(expected, from.ToString());
		}

		[Fact]
		public void ToString_WithJoin_GeneratesCorrectSQLClause()
		{
			Tables_Blogs blogs = new Tables_Blogs();
			Tables_Posts posts = new Tables_Posts();

			FromClause from = new FromClause(blogs).Join(posts, blogs.Id == posts.BlogId );

			string expected = "FROM ([dbo].[Blogs] JOIN [dbo].[Posts] ON ([dbo].[Blogs].[Id]=[dbo].[Posts].[BlogId])) ";

			Assert.Equal(expected, from.ToString());
		}
	}
}