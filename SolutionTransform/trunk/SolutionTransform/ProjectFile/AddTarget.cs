// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace SolutionTransform.ProjectFile
{
	using System.Xml;

	public class AddTarget : MSBuild2003Transform
	{
		private readonly string target;

		public AddTarget(string target)
		{
			this.target = target;
		}

		public override void DoApplyTransform(XmlDocument document)
		{
			var import = AddElement(document.DocumentElement, "Import");
			import.SetAttribute("Condition", "");
			import.SetAttribute("Project", target);
		}
	}
}
