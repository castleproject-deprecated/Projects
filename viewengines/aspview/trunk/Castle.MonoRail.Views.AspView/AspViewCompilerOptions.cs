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
		private bool _autoRecompilation = false;
		private bool _keepTemporarySourceFiles = false;
		private string _temporarySourceFilesDirectory = "temporarySourceFiles";
		private List<ReferencedAssembly> _assembliesToReference = new List<ReferencedAssembly>();

		static readonly ReferencedAssembly[] defaultAssemblies = new ReferencedAssembly[4]
			{
				new ReferencedAssembly("System.dll", ReferencedAssembly.AssemblySource.GlobalAssemblyCache),
				new ReferencedAssembly("Castle.Core.dll", ReferencedAssembly.AssemblySource.BinDirectory),
				new ReferencedAssembly("Castle.MonoRail.Views.AspView.dll", ReferencedAssembly.AssemblySource.BinDirectory),
				new ReferencedAssembly("Castle.MonoRail.Framework.dll", ReferencedAssembly.AssemblySource.BinDirectory)
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
			bool? autoRecompilation,
			string temporarySourceFilesDirectory,
			bool? keepTemporarySourceFiles,
			IEnumerable<ReferencedAssembly> references)
			: this()
		{
			if (debug.HasValue) _debug = debug.Value;
			if (inMemory.HasValue) _inMemory = inMemory.Value;
			if (autoRecompilation.HasValue) _autoRecompilation = autoRecompilation.Value;
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
		/// if true, the engine will recompile the view if the view sources are changed
		/// </summary>
		public bool AutoRecompilation
		{
			get { return _autoRecompilation; }
			set { _autoRecompilation = value; }
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
		public ReferencedAssembly[] References
		{
			get { return _assembliesToReference.ToArray(); }
		}
		#endregion

		public void AddReferences(IEnumerable<ReferencedAssembly> referencesToAdd)
		{
			_assembliesToReference.AddRange(referencesToAdd);
		}

	}
}
