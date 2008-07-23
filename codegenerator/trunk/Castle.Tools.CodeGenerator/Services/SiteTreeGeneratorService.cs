// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.Tools.CodeGenerator.Services
{
	using System.IO;
	using ICSharpCode.NRefactory;

	public class SiteTreeGeneratorService : ISiteTreeGeneratorService
	{
		private readonly ITypeResolver typeResolver;
		private readonly IParsedSourceStorageService cache;
		private readonly IParserFactory parserFactory;

		public SiteTreeGeneratorService(ILogger logger, ITypeResolver typeResolver, IParsedSourceStorageService cache, IParserFactory parserFactory)
		{
			this.typeResolver = typeResolver;
			this.cache = cache;
			this.parserFactory = parserFactory;
		}

		public void Parse(IAstVisitor visitor, string path)
		{
			using (TextReader reader = File.OpenText(path))
			{
				var parser = parserFactory.CreateCSharpParser(reader);
				parser.ParseMethodBodies = true;
				parser.Parse();

				typeResolver.Clear();
				visitor.VisitCompilationUnit(parser.CompilationUnit, null);
				cache.Add(path, parser);
			}
		}
	}
}