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



namespace Castle.MonoRail.Views.AspView.Tests
{
	using System.Text.RegularExpressions;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;

    [TestFixture]
	public class LanguagePreProcessorTestCases
    {/*
        [Test]
		public void ViewComponent_Simple_Matched()
		{
			string input = @"<%@ Page Language=""C#"" %>
<%
%>
some text before viewcomponent
<component:Simple></component:Simple>
some text after viewcomponent
";
			Regex findViewComponentTags = new Regex(LanguagePreProcessor.RegexPatterns.ViewComponentTags, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);

			Assert.That(findViewComponentTags.IsMatch(input), Is.True);

		}

		[Test]
		public void ViewComponent_WithDotInName_Matched()
        {
			string input = @"<%@ Page Language=""C#"" %>
<%
%>
some text before viewcomponent
<component:With.Dot.In.Name></component:With.Dot.In.Name>
some text after viewcomponent
";
			Regex findViewComponentTags = new Regex(LanguagePreProcessor.RegexPatterns.ViewComponentTags, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);

			Assert.That(findViewComponentTags.IsMatch(input), Is.True);

        }

		[Test]
		public void ViewComponent_WithTextParameter_Matched()
		{
			string input = @"<%@ Page Language=""C#"" %>
<%
%>
some text before viewcomponent
<component:Simple param=""text""></component:Simple>
some text after viewcomponent
";
			Regex findViewComponentTags = new Regex(LanguagePreProcessor.RegexPatterns.ViewComponentTags, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);

			Assert.That(findViewComponentTags.IsMatch(input), Is.True);

		}

		[Test]
		public void ViewComponent_WithTextParameterThatIncludesADot_Matched()
		{
			string input = @"<%@ Page Language=""C#"" %>
<%
%>
some text before viewcomponent
<component:Simple param=""text.dot""></component:Simple>
some text after viewcomponent
";
			Regex findViewComponentTags = new Regex(LanguagePreProcessor.RegexPatterns.ViewComponentTags, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);

			Assert.That(findViewComponentTags.IsMatch(input), Is.True);

		}
		/**/
	}
}