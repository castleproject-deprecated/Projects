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

namespace ViewUpgrader.Tests
{
	using Xunit;

	public class TrasformersFixture
	{
		[Fact]
		public void TransformPropertyDecleration_WhenInputIsView_Transforms()
		{
			string input = @"<%@ Page Language=""C#"" Inherits=""Castle.MonoRail.Views.AspView.ViewAtDesignTime"" %>
<%
	int? someIntegerWithDefaultValue = default(int);
%>
<%=someIntegerWithDefaultValue%>
";
			string expected = @"<%@ Page Language=""C#"" Inherits=""Castle.MonoRail.Views.AspView.ViewAtDesignTime"" %>
<aspView:properties>
<%
	int? someIntegerWithDefaultValue = default(int);
%>
</aspView:properties>
<%=someIntegerWithDefaultValue%>
";
			string actual = Transformer.TransformPropertyDecleration(input);
			Assert.Equal(expected, actual);
		}

		[Fact]
		public void TransformSubViewProperties_WhenInputIsView_Transforms()
		{
			string input = @"<%@ Page Language=""C#"" Inherits=""Castle.MonoRail.Views.AspView.ViewAtDesignTime"" %>
<%
    string[] strings;
 %>
hello from index<br />
This are the strings:<br />
<%foreach (string s in strings) { %>
    <%=s %><br />
<%} %>
        
<br />
End of normal view
<br />
<%string message = ""Hello"";%>
<subView:SubViewSample message=""message"" number = ""1"" ></subView:SubViewSample>
<form action=""Print.rails"">
<input type=""text"" name=""theText"" />
<input type=""submit"" value=""send"" />
</form>";
			string expected = @"<%@ Page Language=""C#"" Inherits=""Castle.MonoRail.Views.AspView.ViewAtDesignTime"" %>
<%
    string[] strings;
 %>
hello from index<br />
This are the strings:<br />
<%foreach (string s in strings) { %>
    <%=s %><br />
<%} %>
        
<br />
End of normal view
<br />
<%string message = ""Hello"";%>
<subView:SubViewSample message=""<%=message%>"" number=""<%=1%>""></subView:SubViewSample>
<form action=""Print.rails"">
<input type=""text"" name=""theText"" />
<input type=""submit"" value=""send"" />
</form>";
			string actual = Transformer.TransformSubViewProperties(input);
			Assert.Equal(expected, actual);
		}

		[Fact]
		public void TransformPropertyDecleration_WhenInputIsNotView_ReturnsInput()
		{
			string input = @"Not a view";
			string actual = Transformer.TransformPropertyDecleration(input);
			Assert.Equal(input, actual);
		}

		[Fact]
		public void TransformPropertyDecleration_WhenInputHasAlreadyBeenTransformed_ReturnsInput()
		{
			string input = @"<%@ Page Language=""C#"" Inherits=""Castle.MonoRail.Views.AspView.ViewAtDesignTime"" %>
<aspView:properties>
<%
	int? someIntegerWithDefaultValue = default(int);
%>
</aspView:properties>
<%=someIntegerWithDefaultValue%>
";
			string actual = Transformer.TransformPropertyDecleration(input);
			Assert.Equal(input, actual);
		}

		[Fact]
		public void TransformSubViewProperties_WhenInputIsNotView_ReturnsInput()
		{
			string input = @"Not a view";
			string actual = Transformer.TransformSubViewProperties(input);
			Assert.Equal(input, actual);
		}

		[Fact]
		public void TransformSubViewProperties_WhenInputHasAlreadyBeenTransformed_Transforms()
		{
			string input = @"<%@ Page Language=""C#"" Inherits=""Castle.MonoRail.Views.AspView.ViewAtDesignTime"" %>
<%
    string[] strings;
 %>
hello from index<br />
This are the strings:<br />
<%foreach (string s in strings) { %>
    <%=s %><br />
<%} %>
        
<br />
End of normal view
<br />
<%string message = ""Hello"";%>
<subView:SubViewSample message=""<%=message%>"" number=""<%=1%>""></subView:SubViewSample>
<form action=""Print.rails"">
<input type=""text"" name=""theText"" />
<input type=""submit"" value=""send"" />
</form>";
			string actual = Transformer.TransformSubViewProperties(input);
			Assert.Equal(input, actual);
		}
	}
}
