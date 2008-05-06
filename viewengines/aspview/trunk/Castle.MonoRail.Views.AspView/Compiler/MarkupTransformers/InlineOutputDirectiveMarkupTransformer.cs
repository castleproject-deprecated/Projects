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

namespace Castle.MonoRail.Views.AspView.Compiler.MarkupTransformers
{
	using System.Text.RegularExpressions;

	public class InlineOutputDirectiveMarkupTransformer : IMarkupTransformer
	{

		public string Transform(string markup)
		{
			return Internal.RegularExpressions.InlineOutputDirective.Replace(markup, delegate(Match match)
			{
				string content = match.Groups["content"].Value;
				return string.Format("<% OutputEncoded({0}); %>", content);
			});
		}
	}
}
