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

namespace Castle.MonoRail.Views.AspView.Compiler.PreCompilationSteps
{
	using System.Collections.Generic;
	using System.Text.RegularExpressions;

	public class ViewComponentTagsStep : IPreCompilationStep
	{
		static int lastViewComponentsCounter;

		readonly ScriptToCodeTransformer scriptTransformer = new ScriptToCodeTransformer();

		public void Process(SourceFile file)
		{
			file.RenderBody = Process(file.RenderBody, file);
		}

		private string Process(string currentContent, SourceFile file)
		{
			return Internal.RegularExpressions.ViewComponentTags.Replace(currentContent, delegate(Match match)
			{
				string componentTypeName = match.Groups["componentName"].Value;
				string attributes = match.Groups["attributes"].Value;
				string content = match.Groups["content"].Value;

				int viewComponentsCounter = lastViewComponentsCounter++;
				int sectionsCounter = 0;
				string parametersString = Utilities.GetAttributesStringFrom(attributes);

				string componentName = (componentTypeName + viewComponentsCounter).Replace('.', '_');

				string bodyHandlerName = null;
				string bodyHandlerParameterString = "null";
				if (content.Trim().Length > 0)
					bodyHandlerName = componentName + "_body";
				if (bodyHandlerName != null)
				{
					RegisterSectionHandler(bodyHandlerName, content, file);
					bodyHandlerParameterString = GetDelegateString(bodyHandlerName);
				}

				List<string> pairs = new List<string>();

				foreach (Match sectionTag in Internal.RegularExpressions.ViewComponentSectionTags.Matches(content))
				{
					++sectionsCounter;
					string sectionTypeName = sectionTag.Groups["sectionName"].Value;
					string sectionContent = sectionTag.Groups["content"].Value;
					string sectionName = sectionTypeName + sectionsCounter;
					string handlerName = componentName + "_" + sectionName;
					RegisterSectionHandler(handlerName, sectionContent, file);
					pairs.Add(string.Format(@"new KeyValuePair<string, object>(""{0}"", {1}) ",
						sectionTypeName, GetDelegateString(handlerName)));
				}

				string sectionHandlersArray = string.Format(
					@"new KeyValuePair<string, object>[] {{ {0} }}",
					string.Join(", ", pairs.ToArray()));

				return string.Format(
					@"<% InvokeViewComponent(""{0}"", {1}, {2} {3}); %>",
					componentTypeName, bodyHandlerParameterString, sectionHandlersArray, parametersString);
			});
		}

		private static string GetDelegateString(string handlerName)
		{
			return string.Format(
				@"new ViewComponentSectionRendereDelegate({0})", handlerName); 
		}

		private void RegisterSectionHandler(string handlerName, string sectionContent, SourceFile file)
		{
			string processedSection = sectionContent;
			if (Internal.RegularExpressions.ViewComponentTags.IsMatch(sectionContent))
				processedSection = Process(sectionContent, file);
			processedSection = scriptTransformer.Transform(processedSection);
			file.ViewComponentSectionHandlers[handlerName] = processedSection;
		}
	}
}
