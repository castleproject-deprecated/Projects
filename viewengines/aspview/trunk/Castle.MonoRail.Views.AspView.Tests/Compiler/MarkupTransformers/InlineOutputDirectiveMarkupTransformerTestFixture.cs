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

using Castle.MonoRail.Views.AspView.Compiler.StatementProcessors.OutputMethodGenerators;

namespace Castle.MonoRail.Views.AspView.Tests.Compiler.MarkupTransformers
{
	using Xunit;
	using AspView.Compiler.MarkupTransformers;
	
	
	public class InlineOutputDirectiveMarkupTransformerTestFixture : AbstractMarkupTransformerTestFixture
	{
		protected override void CreateTransformer()
		{
			transformer = new InlineOutputDirectiveMarkupTransformer(new EncodedOutputMethodGenerator());
		}

		[Fact]
		public void Transform_WhenNoDirective_DoNothing()
		{
			input = "Some stuff";
			expected = input;
		
			AssertTranfromerOutput();
		}

		[Fact]
		public void Transform_OnlyDirective_TransformsWithNullCheck()
		{
			input = "${someVar}";
			expected = "<% OutputEncoded((someVar == null) ? string.Empty : someVar.ToString()); %>";
			AssertTranfromerOutput();
		}

		[Fact]
		public void Transform_WhenDirectiveAndRegularMarkup_Transforms()
		{
			input = "this is my ${name}.";
			expected = "this is my <% OutputEncoded((name == null) ? string.Empty : name.ToString()); %>.";
			AssertTranfromerOutput();
		}
	}
}
