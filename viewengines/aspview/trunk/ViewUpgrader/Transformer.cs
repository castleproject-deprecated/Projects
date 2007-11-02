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

namespace ViewUpgrader
{
	using System;
	using System.Collections.Generic;
	using System.Text.RegularExpressions;

	public static class Transformer
	{
		private static readonly Regex findPageDirective = new Regex("<%@\\s*Page\\s+Language\\s*=\\s*\"(?<language>[\\w#+]+)\"(?:\\s+Inherits\\s*=\\s*\"[\\w.]+<(?<view>[\\w.]+)>\\s*\")?.*%>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		private static readonly Regex findSubViewTags = new Regex("<subView:(?<viewName>[\\w\\.]+)\\s*(?<attributes>[\\w\"\\s=]*)\\s*>\\s*</subView:\\k<viewName>>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		private static readonly Regex findAttributes = new Regex("\\s*(?<name>\\w+)\\s*=\\s*\"(?<value>[\\w.]*|\\s*<%=\\s*[\\w\\.\\(\\)\"]+\\s*%>\\s*)\"\\s*");

		public static string TransformPropertyDecleration(string input)
		{
			if (!IsView(input))
				return input;

			if (AlreadyHasPropertyDecleration(input))
				return input;


			List<string> lines = new List<string>(input.Split(new string[] { Environment.NewLine }, StringSplitOptions.None));
			int lineIndex = 0;
			string line = lines[lineIndex];

			while (line.Trim() != "<%")
				line = lines[++lineIndex];

			lines.Insert(lineIndex++, "<aspView:properties>");

			while (line.Trim() != "%>")
				line = lines[++lineIndex];

			lines.Insert(++lineIndex, "</aspView:properties>");

			return string.Join(Environment.NewLine, lines.ToArray());
		}



		public static string TransformSubViewProperties(string input)
		{
			if (!IsView(input))
				return input;

			string output = findSubViewTags.Replace(input, SubViewTagHandler);

			return output;
		}
		private static string SubViewTagHandler(Match match)
		{
			string subViewDecleration = match.Value;
			string newDecleration = findAttributes.Replace(subViewDecleration, SubViewAttributeTagHandler);
			return newDecleration;
		}
		private static string SubViewAttributeTagHandler(Match match)
		{
			string name = match.Groups["name"].Value;
			string value = match.Groups["value"].Value;
			return string.Format(" {0}=\"<%={1}%>\"", name, value);
		}

		private static bool IsView(string input)
		{
			return findPageDirective.IsMatch(input);
		}

		private static bool AlreadyHasPropertyDecleration(string input)
		{
			return input.Contains("<aspView:properties>");
		}
	}
}
