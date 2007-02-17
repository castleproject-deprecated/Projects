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
		private bool _debug = false;
		private bool _inMemory = false;
		private bool _keepTemporarySourceFiles = false;
		private string _temporarySourceFilesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temporarySourceFiles");
		private List<string> _assembliesToReference = new List<string>();

		static readonly string[] defaultAssemblies = new string[4]
			{
				"System.dll",
				"Castle.Core.dll",
				"Castle.MonoRail.Views.AspView.dll",
				"Castle.MonoRail.Framework.dll"
			};
		#endregion

		#region c'tor
		public AspViewCompilerOptions()
		{
			AddReferences(defaultAssemblies);
		}

		public AspViewCompilerOptions(
			bool? debug,
			bool? inMemory,
			string temporarySourceFilesDirectory,
			bool? keepTemporarySourceFiles,
			IEnumerable<string> references)
			: this()
		{
			if (debug.HasValue) _debug = debug.Value;
			if (inMemory.HasValue) _inMemory = inMemory.Value;
			if (keepTemporarySourceFiles.HasValue) _keepTemporarySourceFiles = keepTemporarySourceFiles.Value;
			if (temporarySourceFilesDirectory != null) _temporarySourceFilesDirectory = temporarySourceFilesDirectory;
			AddReferences(references);
		}
		#endregion

		#region properties
		/// <summary>
		/// True to emit debug symbols
		/// </summary>
		public bool Debug
		{
			get { return _debug; }
			set { _debug = value; }
		}
		/// <summary>
		/// True if the compiled assembly should only be kept in-memory
		/// </summary>
		public bool InMemory
		{
			get { return _inMemory; }
			set { _inMemory = value; }
		}
		/// <summary>
		/// True if the generated concrete classes should be kept on disk
		/// </summary>
		public bool KeepTemporarySourceFiles
		{
			get { return _keepTemporarySourceFiles; }
			set { _keepTemporarySourceFiles = value; }
		}
		/// <summary>
		/// Location of the generated concrete classes, if saved.
		/// Note that the user who runs the application must have Modify permissions on this path.
		/// </summary>
		public string TemporarySourceFilesDirectory
		{
			get { return _temporarySourceFilesDirectory; }
			set { _temporarySourceFilesDirectory = value; }
		}
		/// <summary>
		/// Gets list of assemblies that'll be referenced during the compile process by CompiledViews.dll
		/// </summary>
		public string[] References
		{
			get { return _assembliesToReference.ToArray(); }
		}
		#endregion

		public void AddReferences(IEnumerable<string> referencesToAdd)
		{
			_assembliesToReference.AddRange(referencesToAdd);
		}

	}
}
