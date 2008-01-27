using Castle.Tools.SQLQueryGenerator.Runtime.Clauses;
using Castle.Tools.SQLQueryGenerator.Runtime.Model.Field;
using Castle.Tools.SQLQueryGenerator.Tests.GeneratedClasses;
using Xunit;

namespace Castle.Tools.SQLQueryGenerator.Tests.RuntimeFixtures
{
	public class SelectClauseTests
	{
		[Fact]
		public void ToString_WithSingleField_GeneratesCorrectSQLClause()
		{
			Tables_Blogs table = new Tables_Blogs();

			IFormatableField[] fields = new IFormatableField[]
			{
				new Tables_Blogs_Id(table)
			};

			SelectClause select = new SelectClause(fields);

			string expected = 
@"SELECT
				[dbo].[Blogs].[Id]
";

			Assert.Equal(expected, select.ToString());
		}

		[Fact]
		public void ToString_WithMoreThanOneField_GeneratesCorrectSQLClause()
		{
			Tables_Blogs table = new Tables_Blogs();

			IFormatableField[] fields = new IFormatableField[]
			{
				new Tables_Blogs_Id(table),
				new Tables_Blogs_Name(table)
			};

			SelectClause select = new SelectClause(fields);

			string expected = 
@"SELECT
				[dbo].[Blogs].[Id],
				[dbo].[Blogs].[Name]
";

			Assert.Equal(expected, select.ToString());
		}

		[Fact]
		public void ToString_WithAlias_GeneratesCorrectSQLClause()
		{
			Tables_Blogs table = new Tables_Blogs();

			IFormatableField[] fields = new IFormatableField[]
			{
				new Tables_Blogs_Id(table).As("MyId"),
				new Tables_Blogs_Name(table)
			};

			SelectClause select = new SelectClause(fields);

			string expected = 
@"SELECT
				[dbo].[Blogs].[Id] AS [MyId],
				[dbo].[Blogs].[Name]
";

			Assert.Equal(expected, select.ToString());
		}
	}
}