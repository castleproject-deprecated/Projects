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

namespace Castle.Monorail.Views.NDjangoView
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.IO;
	using System.Web;
	using NDjango.Interfaces;
	
	/// <summary>
    /// Class, which implements ITemplateLoader interface - used by NDjango engine to load templates 
    /// and check - whether template was updated.
    /// </summary>
    internal class TemplateLoader : ITemplateLoader
    {
		/// <summary>
		/// Views directory - templates are stored here.
		/// </summary>
		private string rootDir;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateLoader"/> class.
        /// Sets initial directory, where templates are.
        /// </summary>
        internal TemplateLoader()
        {
            rootDir = HttpRuntime.AppDomainAppPath + "Views\\";
        }

        #region ITemplateLoader Members

        /// <summary>
        /// Gets the template.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public TextReader GetTemplate(string name)
        {
            return File.OpenText(Path.Combine(rootDir, name));
        }

        /// <summary>
        /// Determines whether the specified name is updated.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="timestamp">The timestamp.</param>
        /// <returns>
        /// 	<c>true</c> if the specified name is updated; otherwise, <c>false</c>.
        /// </returns>
        public bool IsUpdated(string name, System.DateTime timestamp)
        {
            return File.GetLastWriteTime(Path.Combine(rootDir, name)) > timestamp;
        }

        #endregion
    }
}
