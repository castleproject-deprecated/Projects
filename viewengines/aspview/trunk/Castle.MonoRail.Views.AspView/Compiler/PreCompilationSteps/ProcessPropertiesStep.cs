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

using System;
using System.Text.RegularExpressions;

namespace Castle.MonoRail.Views.AspView.Compiler.PreCompilationSteps
{
	public class ProcessPropertiesStep : IPreCompilationStep
	{
		public void Process(SourceFile file)
		{
			file.ViewSource = Internal.RegularExpressions.PropertiesSection.Replace(file.ViewSource, delegate(Match match)
			{
				string propertiesSection = match.Groups["properties"].Value.Trim();
				if (propertiesSection.EndsWith("%>"))
					propertiesSection = propertiesSection.Substring(0, propertiesSection.Length - 2);
				string[] propertiesDeclerations = propertiesSection.Split(';');
				foreach (string propertiesDecleration in propertiesDeclerations)
				{
					string prop = propertiesDecleration.Trim();
					if (prop == string.Empty)
						continue;
					string[] mainParts = prop.Split(new char[1] { '=' }, 2);
					string propDecleration = mainParts[0].Trim();
					string defaultValue = null;
					if (mainParts.Length == 2)
						defaultValue = mainParts[1].Trim();
					int lastSpace = propDecleration.LastIndexOf(" ");
					if (lastSpace == -1)
						throw new Exception("Illegal property decleration: '" + prop + "'");
					string type = propDecleration.Substring(0, lastSpace).Trim();
					string name = propDecleration.Substring(lastSpace).Trim();
					file.Properties.Add(name, new ViewProperty(name, type, defaultValue));
				}
				return string.Empty;
			});
		}

	}
}
