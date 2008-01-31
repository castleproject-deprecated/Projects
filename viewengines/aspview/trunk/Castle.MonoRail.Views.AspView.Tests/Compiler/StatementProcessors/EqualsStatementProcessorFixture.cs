#region license
// Copyright 2006-2007 Ken Egozi http://www.kenegozi.com/
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

namespace Castle.MonoRail.Views.AspView.Tests.Compiler.StatementProcessors
{
	using AspView.Compiler.StatementProcessors;
	using AspView.Compiler.StatementProcessors.OutputMethodGenerators;
	using Xunit;

	
	public class EqualsStatementProcessorFixture
	{
		EqualsStatementProcessor processor;

		public EqualsStatementProcessorFixture()
		{
			SetUp();
		}

		public void SetUp()
		{
			processor = new EqualsStatementProcessor();
		}

		[Fact]
		public void CanHandle_WhenStartsWithEqualsSign_Matches()
		{
			string statement = "= foo";
			Assert.True(processor.CanHandle(statement));
		}

		[Fact]
		public void CanHandle_WhenStartsWithWhitespaceAndEqualsSign_Matches()
		{
			string statement = "  = foo";
			Assert.True(processor.CanHandle(statement));
		}

		[Fact]
		public void CanHandle_WhenDoesNotStartsWithArbitraryWhitespaceAndEqualsSign_DoNotMatches()
		{
			string statement = " foo";
			Assert.False(processor.CanHandle(statement));
		}

		[Fact]
		public void GetInfo_WhenStartsWithEqualsSign_GetsCorrectInfo()
		{
			string statement = "= foo";

			string expectedContent = "foo";

			StatementInfo statementInfo = processor.GetInfoFor(statement);

			Assert.Equal(expectedContent, statementInfo.Content);

			Assert.IsType(typeof(OutputMethodGenerator), statementInfo.Generator);
		}

		[Fact]
		public void GetInfo_WhenStartsWithWhitespaceAndEqualsSign_GetsCorrectInfo()
		{
			string statement = "  = foo";

			string expectedContent = "foo";

			StatementInfo statementInfo = processor.GetInfoFor(statement);

			Assert.Equal(expectedContent, statementInfo.Content);

			Assert.IsType(typeof(OutputMethodGenerator), statementInfo.Generator);
		}


	}
}
