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
	using System.Text;

    /// <summary>
    /// Implementation of the django {% url %} tag.
    /// 
    /// This tag will take a url in a String.Format format, and apply the 
    /// supplied parameters to it.
    /// </summary>
    internal class MonorailUrlTag : NDjango.Tags.Abstract.UrlTag
    {
		/// <summary>
		/// Root directory
		/// </summary>
        private string rootDir;

        /// <summary>
        /// Initializes a new instance of the <see cref="MonorailUrlTag"/> class.
        /// </summary>
        /// <param name="rootDir">The application virtual path.</param>
        internal MonorailUrlTag(string rootDir)
        {
            // trim "/" to guarantee it's not there, then add to not to do it every time
            this.rootDir = rootDir.TrimEnd('/') + '/';
        }

        /// <summary>
        /// Generates the URL.
        /// </summary>
        /// <param name="pathTemplate">The path template.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override string GenerateUrl(string pathTemplate, string[] parameters, NDjango.Interfaces.IContext context)
        {
            return rootDir + String.Format(pathTemplate.Trim('/'), parameters);
        }
    }
}
