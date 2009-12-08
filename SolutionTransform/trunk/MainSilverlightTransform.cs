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

namespace SolutionTransform
{
	using System;
	using System.Xml;
	using System.Xml.Linq;

	public class MainSolutionTransform : AltDotNetTransform
	{
		public override void DoApplyTransform(XmlDocument document) {
			base.DoApplyTransform(document);
			var rootPropertyGroup = document.SelectSingleNode("/*/x:PropertyGroup[not(@Condition)]", namespaces);
			if (rootPropertyGroup == null)
			{
				throw new Exception("Couldn't find root property group.");
			}
			Delete(rootPropertyGroup, "x:FileAlignment");
			SetElement(rootPropertyGroup, "SilverlightApplication", false);
			SetElement(rootPropertyGroup, "ValidateXaml", true);
			SetElement(rootPropertyGroup, "ThrowErrorsInValidation", true);
			SetElement(rootPropertyGroup, "ProjectTypeGuids", "{A1591282-1198-4647-A2B1-27E5FF5F6F3B};{fae04ec0-301f-11d3-bf4b-00c04f79efbc}");

			Delete(document, "/*/*/x:Reference/x:RequiredTargetFramework");
			var extensions = AddElement(document.DocumentElement, "ProjectExtensions");
			var vs = AddElement(extensions, "VisualStudio");
			var flavour = AddElement(vs, "FlavorProperties");
			flavour.SetAttribute("GUID", "{A1591282-1198-4647-A2B1-27E5FF5F6F3B}");
			var spp = AddElement(flavour, "SilverlightProjectProperties");
		}
	}
}
