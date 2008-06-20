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

namespace Castle.Tools.SQLQueryGenerator.Tests.RuntimeFixtures
{
	using Runtime.Clauses;
	using Runtime.Model.Field;
	using GeneratedClasses;
	using Xunit;

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