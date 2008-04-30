#region license

// Copyright 2006-2008 Ken Egozi http://www.kenegozi.com/
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

namespace Castle.MonoRail.Views.AspView.Tests.Compiler
{
	using System.CodeDom.Compiler;
	using System.Collections.Generic;
	using System.IO;
	using System.Reflection;
	using Xunit;

	using AspView.Compiler;
	using AspView.Compiler.Adapters;
	using AspView.Compiler.Factories;
	

	public class AspViewCompilerTests
	{
		[Fact]
		public void Compiler_Compiles()
		{
			// ViewSourceLoader 
			// ViewsFolder
			// Target Bin Folder
			// 

			ICodeProviderAdapterFactory codeProviderAdapterFactory = new MockedCodeProviderAdapterFactory();
			IPreProcessor preProcessor = new MockedPreProcessor();
			ICompilationContext context = new CompilationContext(
				new DirectoryInfo("C:\\"), 
				new DirectoryInfo("C:\\"), 
				new DirectoryInfo("C:\\"), 
				new DirectoryInfo("C:\\"));
			AspViewCompilerOptions options = new AspViewCompilerOptions();

			OnlineCompiler compiler = new OnlineCompiler(
				codeProviderAdapterFactory, preProcessor, context, options);

			Assembly compiledViews = compiler.Execute();


		}

		private class MockedPreProcessor : IPreProcessor
		{
			public void ApplyPreCompilationStepsOn(IEnumerable<SourceFile> files)
			{
			}
		}

		private class MockedCodeProviderAdapterFactory : ICodeProviderAdapterFactory
		{
			public ICodeProviderAdapter GetAdapter()
			{
				return new MockedCodeProvider();
			}
		}

		private class MockedCodeProvider : ICodeProviderAdapter
		{
			public CompilerResults CompileAssemblyFromSource(CompilerParameters parameters, params string[] sources)
			{
				return new CompilerResults(new TempFileCollection());
			}

			public CompilerResults CompileAssemblyFromFile(CompilerParameters parameters, params string[] fileNames)
			{
				return new CompilerResults(new TempFileCollection());
			}
		}
	}
}
