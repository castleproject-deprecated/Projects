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

namespace Castle.MonoRail.Views.AspView.Tests.Compiler.PreCompilationSteps
{
	using AspView.Compiler;
	using AspView.Compiler.PreCompilationSteps;
	using Xunit;

	
	public class ProcessPropertiesStepTestFixture : AbstractPreCompilationStepTestFixture
	{
		private static void AssertPropertiesSectionHasBeenRemoved(string viewSource)
		{
			if (Internal.RegularExpressions.PropertiesSection.IsMatch(viewSource))
				throw new AssertException("Properties section has not been removed from view source");
		}

		protected override void CreateStep()
		{
			step = new ProcessPropertiesStep();
		}

		[Fact]
		public void Process_WhenEmpty_DoesNotRegisterAnyProperties()
		{
			file.RenderBody = @"
<aspview:properties>
</aspview:properties>
view content";
			step.Process(file);

			Assert.Equal(0, file.Properties.Count);

			AssertPropertiesSectionHasBeenRemoved(file.RenderBody);
		}

		[Fact]
		public void Process_WhenHasOnlyScriptMarkers_DoesNotRegisterAnyProperties()
		{
			file.RenderBody = @"
<aspview:properties>
<%
%>
</aspview:properties>
view content";
			step.Process(file);

			Assert.Equal(0, file.Properties.Count);

			AssertPropertiesSectionHasBeenRemoved(file.RenderBody);
		}

		[Fact]
		public void Process_WhenHasRegularProperty_RegistersThatProperty()
		{
			file.RenderBody = @"
<aspview:properties>
<%
string myString;
%>
</aspview:properties>
view content";
			step.Process(file);

			AssertViewPropertyEqual(new ViewProperty("myString", "string", null), "myString");

			AssertPropertiesSectionHasBeenRemoved(file.RenderBody);
		}

		[Fact]
		public void Process_WhenHasTwoProperties_RegistersBoth()
		{
			file.RenderBody = @"
<aspview:properties>
<%
string myString;
int myInt;
%>
</aspview:properties>
view content";
			step.Process(file);

			AssertViewPropertyEqual(new ViewProperty("myString", "string", null), "myString");
			AssertViewPropertyEqual(new ViewProperty("myInt", "int", null), "myInt");

			AssertPropertiesSectionHasBeenRemoved(file.RenderBody);
		}

		[Fact]
		public void Process_WhenHasDefaultValue_RegistersPropertyWithTheValue()
		{
			file.RenderBody = @"
<aspview:properties>
<%
string myString = ""Sample"";
int myInt;
%>
</aspview:properties>
view content";
			step.Process(file);

			AssertViewPropertyEqual(new ViewProperty("myString", "string", "\"Sample\""), "myString");

			AssertPropertiesSectionHasBeenRemoved(file.RenderBody);
		}


		#region helpers
		private void AssertViewPropertyEqual(ViewProperty expectedProperty, string propertyName)
		{
			Assert.True(file.Properties.ContainsKey(propertyName), string.Format("Property [{0}] is missing.", propertyName));
			ViewProperty actual = file.Properties[propertyName];
			Assert.Equal(expectedProperty.Type, actual.Type);
			if (expectedProperty.DefaultValue == null)
				Assert.Null(actual.DefaultValue);
			else
			{
				Assert.NotNull(actual.DefaultValue);
				Assert.Equal(expectedProperty.DefaultValue, actual.DefaultValue);
			}
		}
		#endregion
	}
}
