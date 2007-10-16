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

namespace Castle.MonoRail.Views.AspView
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public class AspViewPreProcessor
    {
		private static readonly Regex findPageDirective = new Regex("<%@\\s*Page\\s+Language\\s*=\\s*\"(?<language>[\\w#+]+)\"(?:\\s+Inherits\\s*=\\s*\"[\\w.]+<(?<view>[\\w.]+)>\\s*\")?.*%>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		readonly Dictionary<ScriptingLanguage, LanguagePreProcessor> languagePreProcessors;

		public AspViewPreProcessor()
		{
			languagePreProcessors = new Dictionary<ScriptingLanguage, LanguagePreProcessor>(2);
			languagePreProcessors.Add(ScriptingLanguage.CSharp, new CSharpPreProcessor());
			languagePreProcessors.Add(ScriptingLanguage.VbNet, new VbPreProcessor());
		}
		private LanguagePreProcessor GetPreProcessor(ScriptingLanguage language)
		{
			LanguagePreProcessor preProcessor = languagePreProcessors[language];
			if (preProcessor == null)
				throw new AspViewException("Unsupported view language '{0}'", language);
			return preProcessor;
		}

		public void Process(List<AspViewFile> files)
		{
			foreach (AspViewFile file in files)
				Process(file);
		}

		public void Process(AspViewFile file)
		{
			Match pageDirective = findPageDirective.Match(file.ViewSource);
			DetermineScriptingLanguage(file, pageDirective.Groups["language"]);
			DetermineTypedViewName(file, pageDirective.Groups["view"]);
			GetPreProcessor(file.Language).Process(file);
		}

		private static void DetermineTypedViewName(AspViewFile file, Group view)
		{
			if (view != null)
				file.TypedViewName = view.Value;
		}

    	private static void DetermineScriptingLanguage(AspViewFile file, Group languageName)
		{
			switch (languageName.Value.ToLower())
			{
				case "c#":
					file.Language = ScriptingLanguage.CSharp;
					break;
				case "vb":
					file.Language = ScriptingLanguage.VbNet;
					break;
				default:
					throw new AspViewException("Unsupported view language [{0}] in view [{1}]", languageName, file.ViewName);
			}
		}

		public static string GetExtension(ScriptingLanguage language)
		{
			if (language == ScriptingLanguage.CSharp)
				return ".cs";
			if (language == ScriptingLanguage.VbNet)
				return ".vb";
			throw new AspViewException("Unsupported language '{0}'", language);
		}
    }
}
namespace a
{
	using System.Text.RegularExpressions;
	public class test
	{
		public void b()
		{
			string reg = "<%@\\s*Page\\s+Language\\s*=\\s*\"(?<language>[\\w#+]+)\"(?:\\s+Inherits\\s*=\\s*\"[\\w.]+<(?<view>[\\w.]+)>\\s*\")?.*%>";
			Regex findPageDirective = new Regex(reg, RegexOptions.Compiled | RegexOptions.IgnoreCase);

			string header1 = @"<%@ Page Language=""C#"" Inherits=""Castle.MonoRail.Views.AspView.ViewAtDesignTime<AspViewTestSite.Interfaces.UsingDictionaryAdapter.IWithTypedPropertiesView>"" %>
<%@ Import Namespace=""TestModel"" %>";

			string header2 = @"<%@ Page Language=""C#"" Inherits=""Castle.MonoRail.Views.AspView.ViewAtDesignTime"" %>
<%
			%>";

			string header3 = @"<%@ Page Language=""C#"" %>
<%
			%>";

			Match match = findPageDirective.Match(header1);
			System.Console.WriteLine("lang:[{0}]\tview:[{1}]",
				match.Groups["language"].Value,
				match.Groups["view"] == null ? "NULL" : match.Groups["view"].Value);
			match = findPageDirective.Match(header2);
			System.Console.WriteLine("lang:[{0}]\tview:[{1}]",
				match.Groups["language"].Value,
				match.Groups["view"] == null ? "NULL" : match.Groups["view"].Value);
			match = findPageDirective.Match(header3);
			System.Console.WriteLine("lang:[{0}]\tview:[{1}]",
				match.Groups["language"].Value,
				match.Groups["view"] == null ? "NULL" : match.Groups["view"].Value);
		}
	}
}