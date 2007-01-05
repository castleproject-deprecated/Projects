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
    using System.Collections.Specialized;
    using System.IO;

    public class AspViewCompilerOptions
    {
        #region members
        private bool _debug;
        private bool _inMemory;
        private bool _keepTemporarySourceFiles;
        private string[] _references;
        #endregion

        #region c'tor
        public AspViewCompilerOptions(
            bool debug, bool inMemory, bool keepTemporarySourceFiles)
        {
            _debug = debug;
            _inMemory = inMemory;
            _keepTemporarySourceFiles = keepTemporarySourceFiles;
        }
        public AspViewCompilerOptions(
            bool debug, bool inMemory, bool keepTemporarySourceFiles, string[] references)
            :
            this(debug, inMemory, keepTemporarySourceFiles)
        {
            _references = references;
        }
        #endregion

        #region properties
        /// <summary>
        /// True to emit debug symbols
        /// </summary>
        public bool Debug
        {
            get { return _debug; }
        }
        /// <summary>
        /// True if the compiled assembly should only be kept in-memory
        /// </summary>
        public bool InMemory
        {
            get { return _inMemory; }
        }
        /// <summary>
        /// True if the generated concrete classes should be kept on disk
        /// </summary>
        public bool KeepTemporarySourceFiles
        {
            get { return _keepTemporarySourceFiles; }
        }
        /// <summary>
        /// Gets list of assemblies that'll be referenced during the compile process by CompiledViews.dll
        /// </summary>
        public string[] References
        {
            get { return _references; }
        }
        #endregion

    }
}
