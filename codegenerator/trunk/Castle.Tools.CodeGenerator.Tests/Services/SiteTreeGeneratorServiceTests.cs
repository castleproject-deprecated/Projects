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
	using ICSharpCode.NRefactory.Ast;
	using NUnit.Framework;
	using Rhino.Mocks;
	using Rhino.Mocks.Constraints;

	[TestFixture]
	public class SiteTreeGeneratorServiceTests
	{
		private MockRepository mocks;
		private SiteTreeGeneratorService service;
		private ILogger logger;
		private ITypeResolver typeResolver;
		private IAstVisitor visitor;
		private IParser parser;
		private IParserFactory parserFactory;
		private string path;
		private IParsedSourceStorageService sources;

		[SetUp]
		public void Setup()
		{
			mocks = new MockRepository();
			logger = new NullLogger();
			typeResolver = mocks.DynamicMock<ITypeResolver>();
			visitor = mocks.DynamicMock<IAstVisitor>();
			sources = mocks.DynamicMock<IParsedSourceStorageService>();
			parserFactory = mocks.DynamicMock<IParserFactory>();
			parser = mocks.DynamicMock<IParser>();
			service = new SiteTreeGeneratorService(logger, typeResolver, sources, parserFactory);
			path = "~~TemporarySource.cs";
			WriteSampleSource(path);
		}

		[TearDown]
		public void Teardown()
		{
			File.Delete(path);
		}

		[Test]
		public void Parse()
		{
			var unit = new CompilationUnit();

			using (mocks.Unordered())
			{
				Expect.Call(parserFactory.CreateCSharpParser(null)).Constraints(Is.NotNull()).Return(parser);
				parser.ParseMethodBodies = true;
				parser.Parse();
				typeResolver.Clear();
				Expect.Call(parser.CompilationUnit).Return(unit);
				Expect.Call(visitor.VisitCompilationUnit(unit, null)).Return(null);
				sources.Add(path, parser);
			}

			mocks.ReplayAll();
			service.Parse(visitor, path);
			mocks.VerifyAll();
		}

		protected static void WriteSampleSource(string path)
		{
			using (var writer = File.CreateText(path))
			{
				writer.WriteLine("using System;");
				writer.WriteLine("public class Program {");
				writer.WriteLine("public static void Main(string[] args) { }");
				writer.WriteLine("}");
			}
		}
	}
}