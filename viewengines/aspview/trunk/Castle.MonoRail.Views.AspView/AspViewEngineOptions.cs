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

using Castle.MonoRail.Views.AspView.Compiler;

namespace Castle.MonoRail.Views.AspView
{
    public class AspViewEngineOptions
    {
		private string _actionExtension;
		readonly AspViewCompilerOptions _compilerOptions;

		public AspViewEngineOptions()
		{
			_actionExtension = ".aspx";
			_compilerOptions = new AspViewCompilerOptions();
		}
		public AspViewEngineOptions(string actionExtension, AspViewCompilerOptions compilerOptions)
		{
			_actionExtension = actionExtension;
			_compilerOptions = compilerOptions;
		}

		/// <summary>
        /// The actions extension (the virtual extension from the urls)
        /// </summary>
		public string ActionExtension
        {
			get { return _actionExtension; }
			set { _actionExtension = value; }
		}
		/// <summary>
		/// The compiler's options object
		/// </summary>
		public AspViewCompilerOptions CompilerOptions
		{
			get { return _compilerOptions; }
		}		
    }
}
