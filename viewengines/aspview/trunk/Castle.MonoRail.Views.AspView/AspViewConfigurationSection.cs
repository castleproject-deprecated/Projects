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
            AspViewEngineOptions options = new AspViewEngineOptions();

            if (section.Attributes["saveToDisk"] != null)
                options.SaveToDisk = bool.Parse(section.Attributes["saveToDisk"].Value);
            if (section.Attributes["debug"] != null)
                options.Debug = bool.Parse(section.Attributes["debug"].Value);
			if (section.Attributes["siteRoot"] != null)
				options.SiteRoot = section.Attributes["siteRoot"].Value;
			if (section.Attributes["actionExtension"] != null)
			{
				string actionExtension = section.Attributes["actionExtension"].Value;
				if (actionExtension[0] != '.')
					actionExtension = "." + actionExtension;
				options.ActionExtension = actionExtension;
			}
			foreach (XmlNode reference in section.SelectNodes("reference"))
            {
                options.AssembliesToReference.Add(reference.Attributes["assembly"].Value);
            }
            return options;

        }

        #endregion
    }
}