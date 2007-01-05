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
    using System.Reflection;
    using Castle.MonoRail.Framework;

    public class AspViewEngineOptions
    {
        #region members
        private string _siteRoot;
        private bool _saveToDisk;
        private bool _debug;
        private List<string> _assembliesToReference = new List<string>();
		private string _actionExtension;
        #endregion

        public AspViewEngineOptions()
        {
            _saveToDisk = false;
            _debug = false;
            _siteRoot = null;
            _assembliesToReference.Add("System.dll");
            _assembliesToReference.Add("Castle.Core.dll");
            _assembliesToReference.Add("Castle.MonoRail.Views.AspView.dll");
            _assembliesToReference.Add("Castle.MonoRail.Framework.dll");

        }

        /// <summary>
        /// the virtual site's root
        /// </summary>
        public string SiteRoot
        {
            get { return _siteRoot; }
            set { _siteRoot = value; }
        }
        /// <summary>
        /// The actions extension (the virtual extension from the urls)
        /// </summary>
		public string ActionExtension
        {
			get { return _actionExtension; }
			set { _actionExtension = value; }
        }
		
        public bool SaveToDisk
        {
            get { return _saveToDisk; }
            set { _saveToDisk = value; }
        }
        /// <summary>
        /// True if the compiler should emit debug symbols
        /// </summary>
        public bool Debug
        {
            get { return _debug; }
            set { _debug = value; }
        }
        /// <summary>
        /// Assemblies to link to the CompiledViews assembly. This feature isn't yet implemented
        /// </summary>
        public List<string> AssembliesToReference
        {
            get { return _assembliesToReference; }
        }
    }
}
