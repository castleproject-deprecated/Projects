// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;
    using Castle.MonoRail.Framework.Views;
    using System.Collections.Specialized;
    using System.IO;
    using Castle.MonoRail.Framework;

    public class AspViewPreProcessor
    {
		private static readonly Regex findPageDirective = new Regex("<%@\\s*Page\\s+Language\\s*=\\s*\"(?<language>[\\w#+]+)\".*%>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		private Dictionary<ScriptingLanguage, LanguagePreProcessor> languagePreProcessors;

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
			DetermineScriptingLanguage(file);
			GetPreProcessor(file.Language).Process(file);
		}

		private void DetermineScriptingLanguage(AspViewFile file)
		{
			string languageName = findPageDirective.Match(file.ViewSource).Groups["language"].Value.ToLower();
			switch (languageName)
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
