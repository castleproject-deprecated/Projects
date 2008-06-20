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
	using GeneratedClasses;
	using Xunit;

	public class FromClauseTests
	{
		[Fact]
		public void ToString_Always_GeneratesCorrectSQLClause()
		{
			Tables_Blogs table = new Tables_Blogs();

			FromClause from = new FromClause(table);

			string expected = 
@"FROM
				[dbo].[Blogs]
";

			Assert.Equal(expected, from.ToString());
		}

		[Fact]
		public void ToString_WithAlias_GeneratesCorrectSQLClause()
		{
			Tables_Blogs table = new Tables_Blogs().As("MyTable");

			FromClause from = new FromClause(table);

			string expected = 
@"FROM
				[dbo].[Blogs] AS [MyTable]
";

			Assert.Equal(expected, from.ToString());
		}

		[Fact]
		public void ToString_WithJoin_GeneratesCorrectSQLClause()
		{
			Tables_Blogs blogs = new Tables_Blogs();
			Tables_Posts posts = new Tables_Posts();

			FromClause from = new FromClause(blogs).Join(posts, blogs.Id == posts.BlogId );

			string expected = 
@"FROM
				[dbo].[Blogs]
	JOIN		[dbo].[Posts] ON
					([dbo].[Blogs].[Id] = [dbo].[Posts].[BlogId])
";

			Assert.Equal(expected, from.ToString());
		}
	}
}