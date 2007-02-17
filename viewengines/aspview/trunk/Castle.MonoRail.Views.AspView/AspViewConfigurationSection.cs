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
    using System.Configuration;
    using System.Xml;
    using System.Reflection;

    public class AspViewConfigurationSection : IConfigurationSectionHandler
    {
        #region IConfigurationSectionHandler Members

        public object Create(object parent, object configContext, XmlNode section)
        {
			bool? debug = null;
			bool? inMemory = null;
			string temporarySourceFilesDirectory = null;
			bool? saveFiles = null;
			List<string> references = new List<string>();
			string actionExtension = null;

			if (section.Attributes["debug"] != null)
				debug = bool.Parse(section.Attributes["debug"].Value);
			if (section.Attributes["inMemory"] != null)
				inMemory = bool.Parse(section.Attributes["inMemory"].Value);
			if (section.Attributes["temporarySourceFilesDirectory"] != null)
				temporarySourceFilesDirectory = section.Attributes["temporarySourceFilesDirectory"].Value;
			if (section.Attributes["saveFiles"] != null)
				saveFiles = bool.Parse(section.Attributes["saveFiles"].Value);
			foreach (XmlNode reference in section.SelectNodes("reference"))
				references.Add(reference.Attributes["assembly"].Value);

			AspViewCompilerOptions compilerOptions = new AspViewCompilerOptions(
				debug, inMemory, temporarySourceFilesDirectory, saveFiles, references);

			if (section.Attributes["actionExtension"] != null)
			{
				actionExtension = section.Attributes["actionExtension"].Value;
				if (actionExtension[0] != '.')
					actionExtension = "." + actionExtension;
			}

			AspViewEngineOptions options = new AspViewEngineOptions(actionExtension, compilerOptions);

            return options;
        }

        #endregion
    }
}
